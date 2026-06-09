import express from "express";
import path from "path";
import Database from "better-sqlite3";
import { createServer as createViteServer } from "vite";
import { WebSocketServer, WebSocket } from "ws";
import { parse } from "url";

async function startServer() {
  const app = express();
  const PORT = 3000;
  let wss: any;

  app.use(express.json());

  // Database initialization
  const db = new Database("database.db");

  // Create tables
  db.exec(`
    CREATE TABLE IF NOT EXISTS users (
      email TEXT PRIMARY KEY,
      passwordHash TEXT,
      name TEXT,
      role TEXT
    );

    CREATE TABLE IF NOT EXISTS tags (
      id TEXT PRIMARY KEY,
      name TEXT,
      color TEXT
    );

    CREATE TABLE IF NOT EXISTS notes (
      id TEXT PRIMARY KEY,
      content TEXT,
      createdAt TEXT,
      author TEXT,
      linkedToType TEXT,
      linkedToId TEXT,
      pinned INTEGER DEFAULT 0
    );

    CREATE TABLE IF NOT EXISTS documents (
      id TEXT PRIMARY KEY,
      title TEXT,
      fileType TEXT,
      fileSize TEXT,
      uploadedAt TEXT,
      linkedToType TEXT,
      linkedToId TEXT
    );

    CREATE TABLE IF NOT EXISTS interactions (
      id TEXT PRIMARY KEY,
      subject TEXT,
      type TEXT,
      assignee TEXT,
      status TEXT,
      client TEXT,
      date TEXT,
      summary TEXT,
      tagIds TEXT
    );

    CREATE TABLE IF NOT EXISTS contacts (
      id TEXT PRIMARY KEY,
      name TEXT,
      role TEXT,
      company TEXT,
      email TEXT,
      phone TEXT
    );

    CREATE TABLE IF NOT EXISTS entities (
      id TEXT PRIMARY KEY,
      name TEXT,
      industry TEXT,
      tier TEXT,
      location TEXT
    );

    CREATE TABLE IF NOT EXISTS engagements (
      id TEXT PRIMARY KEY,
      title TEXT,
      client TEXT,
      type TEXT,
      budget TEXT,
      startDate TEXT,
      endDate TEXT,
      status TEXT,
      description TEXT
    );

    CREATE TABLE IF NOT EXISTS notifications (
      id TEXT PRIMARY KEY,
      actionUserEmail TEXT,
      actionUserName TEXT,
      actionType TEXT,
      entityId TEXT,
      entityName TEXT,
      entityTier TEXT,
      message TEXT,
      createdAt TEXT
    );

    CREATE TABLE IF NOT EXISTS notification_reads (
      notificationId TEXT,
      userEmail TEXT,
      PRIMARY KEY (notificationId, userEmail)
    );

    CREATE TABLE IF NOT EXISTS audit_logs (
      id TEXT PRIMARY KEY,
      userEmail TEXT,
      userName TEXT,
      userRole TEXT,
      actionType TEXT,
      targetType TEXT,
      targetId TEXT,
      targetName TEXT,
      details TEXT,
      timestamp TEXT
    );
  `);

  // Fallback DB Migration to support soft-delete and notes pinning
  try {
    db.exec("ALTER TABLE users ADD COLUMN suspended INTEGER DEFAULT 0");
  } catch (err) {
    // Column already exists or table is still fresh
  }

  try {
    db.exec("ALTER TABLE notes ADD COLUMN pinned INTEGER DEFAULT 0");
  } catch (err) {
    // Column already exists or table is still fresh
  }

  // Helper to check if user table is empty and seed
  const countUsers = db.prepare("SELECT count(*) as count FROM users").get() as { count: number };
  if (countUsers.count === 0) {
    const hashPassword = (pwd: string): string => {
      let hash = 0;
      const salt = "_crm_salt_2026_";
      const salted = pwd + salt;
      for (let i = 0; i < salted.length; i++) {
        hash = (hash << 5) - hash + salted.charCodeAt(i);
        hash = hash & hash;
      }
      return Math.abs(hash).toString(16);
    };

    const defaultHash = hashPassword("password123");
    db.prepare("INSERT INTO users (email, passwordHash, name, role) VALUES (?, ?, ?, ?)").run(
      "david@enterprise.com",
      defaultHash,
      "David Jenkins",
      "Senior Analyst"
    );
  }

  // Seed tags
  const countTags = db.prepare("SELECT count(*) as count FROM tags").get() as { count: number };
  if (countTags.count === 0) {
    const seedTags = [
      { id: "tag-1", name: "High Priority", color: "red" },
      { id: "tag-2", name: "Follow Up", color: "amber" },
      { id: "tag-3", name: "Technical Audit", color: "blue" },
      { id: "tag-4", name: "Onboarding", color: "emerald" },
      { id: "tag-5", name: "Monthly QBR", color: "purple" }
    ];
    const stmt = db.prepare("INSERT INTO tags (id, name, color) VALUES (?, ?, ?)");
    for (const t of seedTags) {
      stmt.run(t.id, t.name, t.color);
    }
  }

  // Seed notes
  const countNotes = db.prepare("SELECT count(*) as count FROM notes").get() as { count: number };
  if (countNotes.count === 0) {
    const seedNotes = [
      {
        id: "note-1",
        content: "Customer requested prioritising marketing deliverables ahead of technical onboarding.",
        createdAt: "2026-06-05T14:30:00Z",
        author: "Sarah K.",
        linkedToType: "interaction",
        linkedToId: "int-1"
      },
      {
        id: "note-2",
        content: "Deploying server-side layers, database sync tests verified successfully.",
        createdAt: "2026-06-02T10:15:00Z",
        author: "Michael R.",
        linkedToType: "interaction",
        linkedToId: "int-2"
      },
      {
        id: "note-3",
        content: "Prefers email communications over secondary desk dialer logs.",
        createdAt: "2026-06-01T09:00:00Z",
        author: "David J.",
        linkedToType: "contact",
        linkedToId: "con-1"
      }
    ];
    const stmt = db.prepare("INSERT INTO notes (id, content, createdAt, author, linkedToType, linkedToId) VALUES (?, ?, ?, ?, ?, ?)");
    for (const n of seedNotes) {
      stmt.run(n.id, n.content, n.createdAt, n.author, n.linkedToType, n.linkedToId);
    }
  }

  // Seed documents
  const countDocs = db.prepare("SELECT count(*) as count FROM documents").get() as { count: number };
  if (countDocs.count === 0) {
    const seedDocs = [
      {
        id: "doc-1",
        title: "Q4 Marketing Milestones Blueprint",
        fileType: "PDF",
        fileSize: "2.4 MB",
        uploadedAt: "2026-06-03T16:00:00Z",
        linkedToType: "interaction",
        linkedToId: "int-1"
      },
      {
        id: "doc-2",
        title: "Enterprise Architecture Core Schema Draft",
        fileType: "Presentation",
        fileSize: "4.8 MB",
        uploadedAt: "2026-05-28T11:45:00Z",
        linkedToType: "interaction",
        linkedToId: "int-2"
      },
      {
        id: "doc-3",
        title: "Apex Solutions Umbrella NDA Signed File",
        fileType: "Contract",
        fileSize: "1.2 MB",
        uploadedAt: "2026-05-15T10:00:00Z",
        linkedToType: "entity",
        linkedToId: "ent-1"
      }
    ];
    const stmt = db.prepare("INSERT INTO documents (id, title, fileType, fileSize, uploadedAt, linkedToType, linkedToId) VALUES (?, ?, ?, ?, ?, ?, ?)");
    for (const d of seedDocs) {
      stmt.run(d.id, d.title, d.fileType, d.fileSize, d.uploadedAt, d.linkedToType, d.linkedToId);
    }
  }

  // Seed interactions
  const countInts = db.prepare("SELECT count(*) as count FROM interactions").get() as { count: number };
  if (countInts.count === 0) {
    const seedInts = [
      {
        id: "int-1",
        subject: "Q4 Marketing Alignment",
        type: "Meeting",
        assignee: "Sarah K.",
        status: "IN PROGRESS",
        client: "Apex Solutions Ltd.",
        date: "2026-06-10",
        summary: "Coordinate Q4 marketing plans, align team on upcoming milestones and core deliverables.",
        tagIds: JSON.stringify(["tag-1", "tag-5"])
      },
      {
        id: "int-2",
        subject: "Enterprise Onboarding Sync",
        type: "Review Session",
        assignee: "Michael R.",
        status: "COMPLETED",
        client: "Stark Enterprises",
        date: "2026-06-01",
        summary: "Successfully reviewed initial corporate server schemas and staging project flow.",
        tagIds: JSON.stringify(["tag-3"])
      },
      {
        id: "int-3",
        subject: "Apex Corp Introductory Meetup",
        type: "Meeting",
        assignee: "David J.",
        status: "SCHEDULED",
        client: "Apex Solutions Ltd.",
        date: "2026-06-15",
        summary: "Initial meeting with corporate stakeholders regarding project guidelines and expectations.",
        tagIds: JSON.stringify(["tag-4"])
      },
      {
        id: "int-4",
        subject: "Project Review Steering Committee",
        type: "Review Session",
        assignee: "Sarah K.",
        status: "BLOCKED",
        client: "Stark Enterprises",
        date: "2026-06-08",
        summary: "Review milestones, risk metrics, and project resource assignments across teams.",
        tagIds: JSON.stringify(["tag-1", "tag-2"])
      }
    ];
    const stmt = db.prepare("INSERT INTO interactions (id, subject, type, assignee, status, client, date, summary, tagIds) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)");
    for (const i of seedInts) {
      stmt.run(i.id, i.subject, i.type, i.assignee, i.status, i.client, i.date, i.summary, i.tagIds);
    }
  }

  // Seed contacts
  const countContacts = db.prepare("SELECT count(*) as count FROM contacts").get() as { count: number };
  if (countContacts.count === 0) {
    const seedContacts = [
      {
        id: "con-1",
        name: "Julianne Apex",
        role: "Stakeholder",
        company: "Apex Solutions Ltd.",
        email: "julianne.a@apex.io",
        phone: "+1 (555) 234-5678"
      },
      {
        id: "con-2",
        name: "Marcus Miller",
        role: "Lead Developer",
        company: "Apex Solutions Ltd.",
        email: "marcus.m@apex.io",
        phone: "+1 (555) 876-5432"
      },
      {
        id: "con-3",
        name: "Sarah Jenkins",
        role: "VP of Partnerships",
        company: "Stark Enterprises",
        email: "sarah.j@stark.com",
        phone: "+1 (555) 432-1098"
      },
      {
        id: "con-4",
        name: "Michael Stark",
        role: "Managing Director",
        company: "Stark Enterprises",
        email: "michael.r@stark.com",
        phone: "+1 (555) 890-1234"
      }
    ];
    const stmt = db.prepare("INSERT INTO contacts (id, name, role, company, email, phone) VALUES (?, ?, ?, ?, ?, ?)");
    for (const c of seedContacts) {
      stmt.run(c.id, c.name, c.role, c.company, c.email, c.phone);
    }
  }

  // Seed entities
  const countEntities = db.prepare("SELECT count(*) as count FROM entities").get() as { count: number };
  if (countEntities.count === 0) {
    const seedEntities = [
      {
        id: "ent-1",
        name: "Apex Solutions Ltd.",
        industry: "Financial Technology",
        tier: "Strategic",
        location: "London, UK"
      },
      {
        id: "ent-2",
        name: "Stark Enterprises",
        industry: "Aerospace & Technology",
        tier: "Enterprise",
        location: "New York, USA"
      },
      {
        id: "ent-3",
        name: "Oscorp Corp",
        industry: "Robotics & Sciences",
        tier: "Key Account",
        location: "Boston, USA"
      }
    ];
    const stmt = db.prepare("INSERT INTO entities (id, name, industry, tier, location) VALUES (?, ?, ?, ?, ?)");
    for (const e of seedEntities) {
      stmt.run(e.id, e.name, e.industry, e.tier, e.location);
    }
  }

  // Seed engagements
  const countEngagements = db.prepare("SELECT count(*) as count FROM engagements").get() as { count: number };
  if (countEngagements.count === 0) {
    const seedEngagements = [
      {
        id: "eng-1",
        title: "FinTech Acceleration SOW",
        client: "Apex Solutions Ltd.",
        type: "SOW Contract",
        budget: "$150,000",
        startDate: "2026-05-01",
        endDate: "2026-11-01",
        status: "Active",
        description: "Implementation of custom server-side database sync layers and dashboard compliance features."
      },
      {
        id: "eng-2",
        title: "AeroTech Marketing Campaign",
        client: "Stark Enterprises",
        type: "Marketing Campaign",
        budget: "$85,000",
        startDate: "2026-06-01",
        endDate: "2026-08-30",
        status: "Under Negotiation",
        description: "Launch promotion campaign for Stark new transport blueprint series."
      },
      {
        id: "eng-3",
        title: "Robotics Strategy Advisory",
        client: "Oscorp Corp",
        type: "Advisory Initiative",
        budget: "$40,000",
        startDate: "2026-07-01",
        endDate: "2026-09-01",
        status: "Pending Draft",
        description: "Providing scientific advisor consultation regarding robotics safety protocols and alignment."
      }
    ];
    const stmt = db.prepare("INSERT INTO engagements (id, title, client, type, budget, startDate, endDate, status, description) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)");
    for (const g of seedEngagements) {
      stmt.run(g.id, g.title, g.client, g.type, g.budget, g.startDate, g.endDate, g.status, g.description);
    }
  }

  // WS clients set & broadcast notification helper
  const broadcastNotification = (notif: any) => {
    if (!wss) return;
    wss.clients.forEach((client: any) => {
      if (client.readyState === 1) { // WebSocket.OPEN
        const clientEmail = client.userEmail;
        const clientRole = client.userRole;

        // Skip broadcasting back to the user who triggered the event
        if (clientEmail === notif.actionUserEmail) {
          return;
        }

        // Permissions check: If tier is Strategic, only Senior Analyst role views it
        if (notif.entityTier === "Strategic" && clientRole !== "Senior Analyst") {
          return;
        }

        client.send(JSON.stringify({
          type: "NOTIFICATION_RECEIVED",
          data: {
            ...notif,
            isRead: 0
          }
        }));
      }
    });
  };

  const addNotification = (
    req: any,
    actionType: "create" | "update" | "delete",
    entityId: string,
    entityName: string,
    entityTier: string,
    message: string
  ) => {
    try {
      const actionUserEmail = req.headers["x-user-email"] || "system@enterprise.com";
      const actionUserName = req.headers["x-user-name"] || "System Operator";
      const id = "notif-" + Date.now() + "-" + Math.random().toString(36).substr(2, 4);
      const createdAt = new Date().toISOString();

      db.prepare(`
        INSERT INTO notifications (id, actionUserEmail, actionUserName, actionType, entityId, entityName, entityTier, message, createdAt)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)
      `).run(id, actionUserEmail, actionUserName, actionType, entityId, entityName, entityTier, message, createdAt);

      const notif = {
        id,
        actionUserEmail,
        actionUserName,
        actionType,
        entityId,
        entityName,
        entityTier,
        message,
        createdAt
      };

      broadcastNotification(notif);
    } catch (err) {
      console.error("Failed to insert and broadcast notification:", err);
    }
  };

  const addAuditLog = (
    req: any,
    actionType: "CREATE" | "UPDATE" | "DELETE" | "BATCH_DELETE" | "BATCH_UPDATE",
    targetType: "Entity" | "Contact" | "Interaction" | "Engagement" | "Note" | "Document" | "User" | "Tag",
    targetId: string,
    targetName: string,
    details: string
  ) => {
    try {
      const userEmail = req.headers["x-user-email"] || "system@enterprise.com";
      const userName = req.headers["x-user-name"] || "System Operator";
      const userRole = req.headers["x-user-role"] || "Operator Unit";
      const id = "audit-" + Date.now() + "-" + Math.random().toString(36).substr(2, 4);
      const timestamp = new Date().toISOString();

      db.prepare(`
        INSERT INTO audit_logs (id, userEmail, userName, userRole, actionType, targetType, targetId, targetName, details, timestamp)
        VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
      `).run(id, userEmail, userName, userRole, actionType, targetType, targetId, targetName, details, timestamp);
    } catch (err) {
      console.error("Failed to insert audit log:", err);
    }
  };

  // API Endpoints: GET ALL STATE
  app.get("/api/all", (req, res) => {
    try {
      const users = db.prepare("SELECT * FROM users").all();
      const tags = db.prepare("SELECT * FROM tags").all();
      const notes = db.prepare("SELECT * FROM notes").all();
      const docs = db.prepare("SELECT * FROM documents").all();
      const ints = db.prepare("SELECT * FROM interactions").all().map((i: any) => ({
        ...i,
        tagIds: i.tagIds ? JSON.parse(i.tagIds) : []
      }));
      const contacts = db.prepare("SELECT * FROM contacts").all();
      const entities = db.prepare("SELECT * FROM entities").all();
      const engagements = db.prepare("SELECT * FROM engagements").all();

      res.json({
        systemUsers: users,
        tags,
        notes,
        documents: docs,
        interactions: ints,
        contacts,
        entities,
        engagements
      });
    } catch (err: any) {
      console.error(err);
      res.status(500).json({ error: err.message });
    }
  });

  // Users Mutator
  app.post("/api/users", (req, res) => {
    try {
      const { email, passwordHash, name, role, suspended } = req.body;
      db.prepare("INSERT OR REPLACE INTO users (email, passwordHash, name, role, suspended) VALUES (?, ?, ?, ?, ?)").run(email, passwordHash, name, role, suspended ?? 0);
      addAuditLog(req, "CREATE", "User", email, name, `Registered or re-created operator user with role: ${role}`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.put("/api/users", (req, res) => {
    try {
      const { email, passwordHash, name, role, suspended } = req.body;
      db.prepare("UPDATE users SET passwordHash = ?, name = ?, role = ?, suspended = ? WHERE email = ?").run(passwordHash, name, role, suspended ?? 0, email);
      addAuditLog(req, "UPDATE", "User", email, name, `Updated operator user profile, new role: ${role} (Suspended status: ${suspended ?? 0})`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.post("/api/users/:email/restore", (req, res) => {
    try {
      const u = db.prepare("SELECT * FROM users WHERE email = ?").get(req.params.email) as any;
      if (u) {
        db.prepare("UPDATE users SET suspended = 0 WHERE email = ?").run(req.params.email);
        addAuditLog(req, "UPDATE", "User", req.params.email, u.name || req.params.email, `Restored and reactivated operator seat`);
        res.json({ success: true });
      } else {
        res.status(404).json({ error: "Operator not found" });
      }
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.delete("/api/users/:email", (req, res) => {
    try {
      const u = db.prepare("SELECT * FROM users WHERE email = ?").get(req.params.email) as any;
      if (u) {
        db.prepare("UPDATE users SET suspended = 1 WHERE email = ?").run(req.params.email);
        addAuditLog(req, "DELETE", "User", req.params.email, u.name || req.params.email, `Soft-deleted / Suspended operator user`);
        res.json({ success: true });
      } else {
        res.status(404).json({ error: "Operator not found" });
      }
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  // Tags Mutator
  app.post("/api/tags", (req, res) => {
    try {
      const { id, name, color } = req.body;
      db.prepare("INSERT OR REPLACE INTO tags (id, name, color) VALUES (?, ?, ?)").run(id, name, color);
      addAuditLog(req, "CREATE", "Tag", id, name, `Created link categorization tag with color: ${color}`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  // Notes Mutator
  app.post("/api/notes", (req, res) => {
    try {
      const { id, content, createdAt, author, linkedToType, linkedToId, pinned } = req.body;
      db.prepare("INSERT OR REPLACE INTO notes (id, content, createdAt, author, linkedToType, linkedToId, pinned) VALUES (?, ?, ?, ?, ?, ?, ?)").run(id, content, createdAt, author, linkedToType, linkedToId, pinned ? 1 : 0);
      addAuditLog(req, "CREATE", "Note", id, content.substr(0, 30) + "...", `Created or updated note attachment (Pinned: ${!!pinned}), linked to ${linkedToType} (#${linkedToId})`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.delete("/api/notes/:id", (req, res) => {
    try {
      const n = db.prepare("SELECT * FROM notes WHERE id = ?").get(req.params.id) as any;
      db.prepare("DELETE FROM notes WHERE id = ?").run(req.params.id);
      if (n) {
        addAuditLog(req, "DELETE", "Note", req.params.id, n.content.substr(0, 30) + "...", `Deleted notes entry belonging to ${n.linkedToType} (#${n.linkedToId})`);
      }
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  // Documents Mutator
  app.post("/api/documents", (req, res) => {
    try {
      const { id, title, fileType, fileSize, uploadedAt, linkedToType, linkedToId } = req.body;
      db.prepare("INSERT OR REPLACE INTO documents (id, title, fileType, fileSize, uploadedAt, linkedToType, linkedToId) VALUES (?, ?, ?, ?, ?, ?, ?)").run(id, title, fileType, fileSize, uploadedAt, linkedToType, linkedToId);
      addAuditLog(req, "CREATE", "Document", id, title, `Uploaded ${fileType} document "${title}" (${fileSize}) linked to ${linkedToType}`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.delete("/api/documents/:id", (req, res) => {
    try {
      const d = db.prepare("SELECT * FROM documents WHERE id = ?").get(req.params.id) as any;
      db.prepare("DELETE FROM documents WHERE id = ?").run(req.params.id);
      if (d) {
        addAuditLog(req, "DELETE", "Document", req.params.id, d.title, `Permanently purged secure archive file "${d.title}" from ${d.linkedToType}`);
      }
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  // Interactions Mutator
  app.post("/api/interactions", (req, res) => {
    try {
      const { id, subject, type, assignee, status, client, date, summary, tagIds } = req.body;
      db.prepare("INSERT OR REPLACE INTO interactions (id, subject, type, assignee, status, client, date, summary, tagIds) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)").run(id, subject, type, assignee, status, client, date, summary, JSON.stringify(tagIds || []));
      addAuditLog(req, "CREATE", "Interaction", id, subject, `Created contact/client interaction "${subject}" with client "${client}"`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.put("/api/interactions/:id", (req, res) => {
    try {
      const { subject, type, assignee, status, client, date, summary, tagIds } = req.body;
      db.prepare("UPDATE interactions SET subject = ?, type = ?, assignee = ?, status = ?, client = ?, date = ?, summary = ?, tagIds = ? WHERE id = ?").run(subject, type, assignee, status, client, date, summary, JSON.stringify(tagIds || []), req.params.id);
      addAuditLog(req, "UPDATE", "Interaction", req.params.id, subject, `Updated interaction "${subject}" attributes (Status: ${status}, Assignee: ${assignee})`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.delete("/api/interactions/:id", (req, res) => {
    try {
      const i = db.prepare("SELECT * FROM interactions WHERE id = ?").get(req.params.id) as any;
      db.prepare("DELETE FROM interactions WHERE id = ?").run(req.params.id);
      if (i) {
        addAuditLog(req, "DELETE", "Interaction", req.params.id, i.subject, `Purged interaction ledger entry for "${i.subject}"`);
      }
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  // Contacts Mutator
  app.post("/api/contacts", (req, res) => {
    try {
      const { id, name, role, company, email, phone } = req.body;
      db.prepare("INSERT OR REPLACE INTO contacts (id, name, role, company, email, phone) VALUES (?, ?, ?, ?, ?, ?)").run(id, name, role, company, email, phone);
      addAuditLog(req, "CREATE", "Contact", id, name, `Registered contact "${name}" (${role} at ${company})`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.put("/api/contacts/:id", (req, res) => {
    try {
      const { name, role, company, email, phone } = req.body;
      db.prepare("UPDATE contacts SET name = ?, role = ?, company = ?, email = ?, phone = ? WHERE id = ?").run(name, role, company, email, phone, req.params.id);
      addAuditLog(req, "UPDATE", "Contact", req.params.id, name, `Modified information for contact "${name}" (${role} at ${company})`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.delete("/api/contacts/:id", (req, res) => {
    try {
      const c = db.prepare("SELECT * FROM contacts WHERE id = ?").get(req.params.id) as any;
      db.prepare("DELETE FROM contacts WHERE id = ?").run(req.params.id);
      if (c) {
        addAuditLog(req, "DELETE", "Contact", req.params.id, c.name, `Deleted contact profile for "${c.name}" (${c.role} at ${c.company})`);
      }
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  // Entities Mutator
  app.post("/api/entities", (req, res) => {
    try {
      const { id, name, industry, tier, location } = req.body;
      db.prepare("INSERT OR REPLACE INTO entities (id, name, industry, tier, location) VALUES (?, ?, ?, ?, ?)").run(id, name, industry, tier, location);
      
      const userName = req.headers["x-user-name"] || "System Operator";
      addNotification(
        req,
        "create",
        id,
        name,
        tier,
        `${userName} created corporate organization "${name}" (${tier} Account)`
      );
      addAuditLog(req, "CREATE", "Entity", id, name, `Created corporate organization "${name}" (${tier} account under ${industry} classification)`);

      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.put("/api/entities/:id", (req, res) => {
    try {
      const { name, industry, tier, location } = req.body;
      db.prepare("UPDATE entities SET name = ?, industry = ?, tier = ?, location = ? WHERE id = ?").run(name, industry, tier, location, req.params.id);
      
      const userName = req.headers["x-user-name"] || "System Operator";
      addNotification(
        req,
        "update",
        req.params.id,
        name,
        tier,
        `${userName} updated corporate organization "${name}" (${tier} Account)`
      );
      addAuditLog(req, "UPDATE", "Entity", req.params.id, name, `Modified corporate organization characteristics for "${name}" (Tier: ${tier})`);

      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.delete("/api/entities/:id", (req, res) => {
    try {
      const ent = db.prepare("SELECT * FROM entities WHERE id = ?").get(req.params.id) as any;
      db.prepare("DELETE FROM entities WHERE id = ?").run(req.params.id);
      
      if (ent) {
        const userName = req.headers["x-user-name"] || "System Operator";
        addNotification(
          req,
          "delete",
          req.params.id,
          ent.name,
          ent.tier,
          `${userName} removed corporate organization "${ent.name}" (${ent.tier} Account)`
        );
        addAuditLog(req, "DELETE", "Entity", req.params.id, ent.name, `Removed corporate organization registry for "${ent.name}" (${ent.tier} Account)`);
      }

      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  // Engagements Mutator
  app.post("/api/engagements", (req, res) => {
    try {
      const { id, title, client, type, budget, startDate, endDate, status, description } = req.body;
      db.prepare("INSERT OR REPLACE INTO engagements (id, title, client, type, budget, startDate, endDate, status, description) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?)").run(id, title, client, type, budget, startDate, endDate, status, description);
      addAuditLog(req, "CREATE", "Engagement", id, title, `Activated brand engagement unit "${title}" with client "${client}" (${type} model)`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.put("/api/engagements/:id", (req, res) => {
    try {
      const { title, client, type, budget, startDate, endDate, status, description } = req.body;
      db.prepare("UPDATE engagements SET title = ?, client = ?, type = ?, budget = ?, startDate = ?, endDate = ?, status = ?, description = ? WHERE id = ?").run(title, client, type, budget, startDate, endDate, status, description, req.params.id);
      addAuditLog(req, "UPDATE", "Engagement", req.params.id, title, `Modified brand engagement structure for "${title}" (Status set to ${status})`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.delete("/api/engagements/:id", (req, res) => {
    try {
      const eg = db.prepare("SELECT * FROM engagements WHERE id = ?").get(req.params.id) as any;
      db.prepare("DELETE FROM engagements WHERE id = ?").run(req.params.id);
      if (eg) {
        addAuditLog(req, "DELETE", "Engagement", req.params.id, eg.title, `Archived or removed brand engagement "${eg.title}" belonging to client "${eg.client}"`);
      }
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  // --- BATCH OPERATIONAL ENDPOINTS ---
  app.post("/api/interactions/batch-delete", (req: any, res: any) => {
    try {
      const { ids } = req.body;
      if (!Array.isArray(ids) || ids.length === 0) {
        return res.status(400).json({ error: "Missing ids array" });
      }
      const placeholders = ids.map(() => "?").join(",");
      db.prepare(`DELETE FROM interactions WHERE id IN (${placeholders})`).run(...ids);
      addAuditLog(req, "BATCH_DELETE", "Interaction", ids.join(", "), `${ids.length} records`, `Performed bulk deletion on ${ids.length} interaction(s)`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.post("/api/interactions/batch-update-status", (req: any, res: any) => {
    try {
      const { ids, status } = req.body;
      if (!Array.isArray(ids) || ids.length === 0 || !status) {
        return res.status(400).json({ error: "Missing ids or status" });
      }
      const placeholders = ids.map(() => "?").join(",");
      db.prepare(`UPDATE interactions SET status = ? WHERE id IN (${placeholders})`).run(status, ...ids);
      addAuditLog(req, "BATCH_UPDATE", "Interaction", ids.join(", "), `${ids.length} records`, `Bulk modified ${ids.length} interaction(s) status to ${status}`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.post("/api/contacts/batch-delete", (req: any, res: any) => {
    try {
      const { ids } = req.body;
      if (!Array.isArray(ids) || ids.length === 0) {
        return res.status(400).json({ error: "Missing ids array" });
      }
      const placeholders = ids.map(() => "?").join(",");
      db.prepare(`DELETE FROM contacts WHERE id IN (${placeholders})`).run(...ids);
      addAuditLog(req, "BATCH_DELETE", "Contact", ids.join(", "), `${ids.length} records`, `Performed bulk profile purge on ${ids.length} contacts`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.post("/api/contacts/batch-update-role", (req: any, res: any) => {
    try {
      const { ids, role } = req.body;
      if (!Array.isArray(ids) || ids.length === 0 || !role) {
        return res.status(400).json({ error: "Missing ids or role" });
      }
      const placeholders = ids.map(() => "?").join(",");
      db.prepare(`UPDATE contacts SET role = ? WHERE id IN (${placeholders})`).run(role, ...ids);
      addAuditLog(req, "BATCH_UPDATE", "Contact", ids.join(", "), `${ids.length} records`, `Bulk refactored role metadata for ${ids.length} contacts to: ${role}`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.post("/api/contacts/batch-update-company", (req: any, res: any) => {
    try {
      const { ids, company } = req.body;
      if (!Array.isArray(ids) || ids.length === 0 || !company) {
        return res.status(400).json({ error: "Missing ids or company" });
      }
      const placeholders = ids.map(() => "?").join(",");
      db.prepare(`UPDATE contacts SET company = ? WHERE id IN (${placeholders})`).run(company, ...ids);
      addAuditLog(req, "BATCH_UPDATE", "Contact", ids.join(", "), `${ids.length} records`, `Bulk updated employer organization for ${ids.length} contacts to: ${company}`);
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });


  // --- AUDIT SYSTEM ENDPOINTS ---
  app.get("/api/audit-logs", (req, res) => {
    try {
      const userRole = (req.headers["x-user-role"] as string) || "";
      if (userRole !== "Senior Analyst") {
        return res.status(403).json({ error: "Access Denied: Administrative Clearance Required" });
      }

      const logs = db.prepare("SELECT * FROM audit_logs ORDER BY timestamp DESC").all();
      res.json(logs);
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.post("/api/audit-logs/purge", (req, res) => {
    try {
      const userRole = (req.headers["x-user-role"] as string) || "";
      if (userRole !== "Senior Analyst") {
        return res.status(403).json({ error: "Access Denied: Administrative Clearance Required" });
      }

      db.prepare("DELETE FROM audit_logs").run();
      addAuditLog(req, "DELETE", "Tag", "all", "Audit Ledger", "Purged entire system audit trails to lock workspace activity history");
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });


  // --- NOTIFICATIONS SYSTEM ENDPOINTS ---
  app.get("/api/notifications", (req, res) => {
    try {
      const userEmail = (req.headers["x-user-email"] as string) || "";
      const userRole = (req.headers["x-user-role"] as string) || "";

      const rows = db.prepare(`
        SELECT n.*, 
               CASE WHEN nr.notificationId IS NOT NULL THEN 1 ELSE 0 END as isRead
        FROM notifications n
        LEFT JOIN notification_reads nr ON n.id = nr.notificationId AND nr.userEmail = ?
        ORDER BY n.createdAt DESC
      `).all(userEmail) as any[];

      // Filter by permissions
      const visibleRows = rows.filter((r) => {
        if (r.entityTier === "Strategic" && userRole !== "Senior Analyst") {
          return false;
        }
        return true;
      });

      res.json(visibleRows);
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.post("/api/notifications/read", (req: any, res: any) => {
    try {
      const userEmail = (req.headers["x-user-email"] as string) || "";
      const { notificationIds } = req.body;
      if (!Array.isArray(notificationIds)) {
        return res.status(400).json({ error: "Missing or invalid notificationIds array" });
      }

      const stmt = db.prepare("INSERT OR IGNORE INTO notification_reads (notificationId, userEmail) VALUES (?, ?)");
      for (const id of notificationIds) {
        stmt.run(id, userEmail);
      }
      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });

  app.post("/api/notifications/read-all", (req, res) => {
    try {
      const userEmail = (req.headers["x-user-email"] as string) || "";
      const userRole = (req.headers["x-user-role"] as string) || "";

      const rows = db.prepare("SELECT id, entityTier FROM notifications").all() as any[];
      const visibleIds = rows
        .filter((r) => !(r.entityTier === "Strategic" && userRole !== "Senior Analyst"))
        .map((r) => r.id);

      const stmt = db.prepare("INSERT OR IGNORE INTO notification_reads (notificationId, userEmail) VALUES (?, ?)");
      for (const id of visibleIds) {
        stmt.run(id, userEmail);
      }

      res.json({ success: true });
    } catch (err: any) {
      res.status(500).json({ error: err.message });
    }
  });


  // Vite development middleware vs Static Production files
  if (process.env.NODE_ENV !== "production") {
    const vite = await createViteServer({
      server: { middlewareMode: true },
      appType: "spa",
    });
    app.use(vite.middlewares);
  } else {
    const distPath = path.join(process.cwd(), "dist");
    app.use(express.static(distPath));
    app.get("*", (req, res) => {
      res.sendFile(path.join(distPath, "index.html"));
    });
  }

  const server = app.listen(PORT, "0.0.0.0", () => {
    console.log(`Server running on http://localhost:${PORT}`);
  });

  // Setup WebSocket Server for real-time notifications
  wss = new WebSocketServer({ server });

  wss.on("connection", (ws: any, req: any) => {
    try {
      const parameters = req.url ? parse(req.url, true).query : {};
      ws.userEmail = parameters.email || "";
      ws.userRole = parameters.role || "";
    } catch (e) {
      console.error("Error setting up WS client details:", e);
    }
  });
}

startServer();
