import React, { useState, useEffect, useMemo } from "react";
import { motion, AnimatePresence } from "motion/react";
import {
  LayoutDashboard,
  Users,
  Search,
  Bell,
  Plus,
  X,
  ChevronRight,
  Trash2,
  Mail,
  Phone,
  MapPin,
  Pin,
  PinOff,
  Clock,
  Tag,
  FileText,
  Paperclip,
  Link,
  Lock,
  LogOut,
  ShieldCheck,
  Check,
  User,
  KeyRound,
  Eye,
  EyeOff,
  Handshake,
  UserCheck,
  Briefcase,
  Notebook,
  Filter,
  ChevronDown,
  Calendar,
  LayoutGrid,
  ChevronLeft,
  ChevronsLeft,
  ChevronsRight,
  ShieldAlert,
  RefreshCw,
  SlidersHorizontal,
  History,
  Shield,
  Download
} from "lucide-react";

// Types
interface CustomTag {
  id: string;
  name: string;
  color: "blue" | "red" | "emerald" | "amber" | "purple" | "indigo";
}

interface Note {
  id: string;
  content: string;
  createdAt: string;
  author: string;
  linkedToType: "interaction" | "contact" | "entity" | "document" | "engagement";
  linkedToId: string;
  pinned?: number | boolean;
}

interface Document {
  id: string;
  title: string;
  fileType: "PDF" | "Spreadsheet" | "Presentation" | "Contract" | "Briefing";
  fileSize: string;
  uploadedAt: string;
  linkedToType: "interaction" | "contact" | "entity" | "engagement";
  linkedToId: string;
}

interface Engagement {
  id: string;
  title: string;
  client: string; // Associated corporate organization name
  type: "SOW Contract" | "Marketing Campaign" | "Retainer" | "Advisory Initiative";
  budget: string;
  startDate: string;
  endDate: string;
  status: "Active" | "Pending Draft" | "Closed" | "Under Negotiation";
  description: string;
}

interface Interaction {
  id: string;
  subject: string;
  type: "Meeting" | "Email Sync" | "Support Call" | "Review Session";
  assignee: string;
  status: "IN PROGRESS" | "COMPLETED" | "SCHEDULED" | "BLOCKED";
  client: string; // Entity name
  date: string;
  summary: string;
  tagIds: string[];
}

interface Contact {
  id: string;
  name: string;
  role: string;
  company: string; // Entity name
  email: string;
  phone: string;
}

interface Entity {
  id: string;
  name: string;
  industry: string;
  tier: "Strategic" | "Enterprise" | "Key Account" | "Growth";
  location: string;
}

interface Toast {
  id: string;
  message: string;
  type: "success" | "info" | "error";
}

// Initial Mock Seed Data
const SEED_TAGS: CustomTag[] = [
  { id: "tag-1", name: "High Priority", color: "red" },
  { id: "tag-2", name: "Follow Up", color: "amber" },
  { id: "tag-3", name: "Technical Audit", color: "blue" },
  { id: "tag-4", name: "Onboarding", color: "emerald" },
  { id: "tag-5", name: "Monthly QBR", color: "purple" }
];

const SEED_NOTES: Note[] = [
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

const SEED_DOCUMENTS: Document[] = [
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

const SEED_INTERACTIONS: Interaction[] = [
  {
    id: "int-1",
    subject: "Q4 Marketing Alignment",
    type: "Meeting",
    assignee: "Sarah K.",
    status: "IN PROGRESS",
    client: "Apex Solutions Ltd.",
    date: "2026-06-10",
    summary: "Coordinate Q4 marketing plans, align team on upcoming milestones and core deliverables.",
    tagIds: ["tag-1", "tag-5"]
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
    tagIds: ["tag-3"]
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
    tagIds: ["tag-4"]
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
    tagIds: ["tag-1", "tag-2"]
  }
];

const SEED_CONTACTS: Contact[] = [
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

const SEED_ENTITIES: Entity[] = [
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

const SEED_ENGAGEMENTS: Engagement[] = [
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

const ASSIGNEES = ["Sarah K.", "Michael R.", "David J.", "Samantha L."];

export default function App() {
  // Session State
  const [session, setSession] = useState<{ email: string; name: string; role: string } | null>(() => {
    const saved = localStorage.getItem("crm_active_session");
    return saved ? JSON.parse(saved) : null;
  });

  const [authScreen, setAuthScreen] = useState<"login" | "signup" | "reset">("login");
  const [authEmail, setAuthEmail] = useState("");
  const [authPassword, setAuthPassword] = useState("");
  const [authConfirmPassword, setAuthConfirmPassword] = useState("");
  const [authName, setAuthName] = useState("");
  const [authRole, setAuthRole] = useState("Senior Analyst");
  const [showPassword, setShowPassword] = useState(false);

  // Persistence States
  const [tags, setTags] = useState<CustomTag[]>(() => {
    const cached = localStorage.getItem("crm_tags");
    return cached ? JSON.parse(cached) : SEED_TAGS;
  });

  const [notes, setNotes] = useState<Note[]>(() => {
    const cached = localStorage.getItem("crm_notes");
    return cached ? JSON.parse(cached) : SEED_NOTES;
  });

  const [documents, setDocuments] = useState<Document[]>(() => {
    const cached = localStorage.getItem("crm_documents");
    return cached ? JSON.parse(cached) : SEED_DOCUMENTS;
  });

  const [interactions, setInteractions] = useState<Interaction[]>(() => {
    const cached = localStorage.getItem("crm_interactions");
    return cached ? JSON.parse(cached) : SEED_INTERACTIONS;
  });

  const [contacts, setContacts] = useState<Contact[]>(() => {
    const cached = localStorage.getItem("crm_contacts");
    return cached ? JSON.parse(cached) : SEED_CONTACTS;
  });

  const [entities, setEntities] = useState<Entity[]>(() => {
    const cached = localStorage.getItem("crm_entities");
    return cached ? JSON.parse(cached) : SEED_ENTITIES;
  });

  const [engagements, setEngagements] = useState<Engagement[]>(() => {
    const cached = localStorage.getItem("crm_engagements");
    return cached ? JSON.parse(cached) : SEED_ENGAGEMENTS;
  });

  const [systemUsers, setSystemUsers] = useState<any[]>(() => {
    const cached = localStorage.getItem("crm_users");
    return cached ? JSON.parse(cached) : [];
  });

  // Navigation and Search
  const [activeTab, setActiveTab] = useState<"dashboard" | "interactions" | "engagements" | "contacts" | "entities" | "notes" | "documents" | "users" | "audit">("dashboard");
  const [searchQuery, setSearchQuery] = useState("");
  const [isSearchActive, setIsSearchActive] = useState(false);

  // System Audit Ledger filter states
  const [auditTargetFilter, setAuditTargetFilter] = useState<string>("ALL");
  const [auditActionFilter, setAuditActionFilter] = useState<string>("ALL");
  const [auditTextSearch, setAuditTextSearch] = useState<string>("");
  const [auditInsightRange, setAuditInsightRange] = useState<"30" | "all">("30");

  // Modal and detail actions
  const [isNewModalOpen, setIsNewModalOpen] = useState(false);
  const [newType, setNewType] = useState<"interaction" | "contact" | "entity" | "engagement" | "user">("interaction");

  const [selectedItem, setSelectedItem] = useState<{
    dataType: "interaction" | "contact" | "entity" | "engagement" | "user";
    data: any;
  } | null>(null);

  // Selected item internal drawer state
  const [drawerTab, setDrawerTab] = useState<"tags" | "notes" | "docs">("tags");
  const [selectedTagToLink, setSelectedTagToLink] = useState("");
  const [newTagName, setNewTagName] = useState("");
  const [newTagColor, setNewTagColor] = useState<CustomTag["color"]>("blue");

  const [newNoteContent, setNewNoteContent] = useState("");
  const [newDocTitle, setNewDocTitle] = useState("");
  const [newDocType, setNewDocType] = useState<Document["fileType"]>("PDF");
  const [newDocSize, setNewDocSize] = useState("");

  // Create entry form states
  const [intForm, setIntForm] = useState({
    subject: "",
    type: "Meeting" as Interaction["type"],
    assignee: ASSIGNEES[0],
    status: "IN PROGRESS" as Interaction["status"],
    client: "",
    date: new Date().toISOString().split("T")[0],
    summary: ""
  });

  const [contactForm, setContactForm] = useState({
    name: "",
    role: "",
    company: "",
    email: "",
    phone: ""
  });

  const [entityForm, setEntityForm] = useState({
    name: "",
    industry: "",
    tier: "Enterprise" as Entity["tier"],
    location: ""
  });

  const [engagementForm, setEngagementForm] = useState({
    title: "",
    client: "",
    type: "SOW Contract" as Engagement["type"],
    budget: "$",
    startDate: new Date().toISOString().split("T")[0],
    endDate: new Date().toISOString().split("T")[0],
    status: "Active" as Engagement["status"],
    description: ""
  });

  const [operatorForm, setOperatorForm] = useState({
    name: "",
    email: "",
    role: "Senior Analyst",
    passphrase: ""
  });

  // Box Selection States for Batch Actions
  const [selectedInteractionIds, setSelectedInteractionIds] = useState<string[]>([]);
  const [selectedContactIds, setSelectedContactIds] = useState<string[]>([]);

  // Interaction Filter States
  const [interactionAssigneeFilter, setInteractionAssigneeFilter] = useState<string>("ALL");
  const [interactionTimeRangeFilter, setInteractionTimeRangeFilter] = useState<string>("ALL");
  const [interactionStartDateFilter, setInteractionStartDateFilter] = useState<string>("");
  const [interactionEndDateFilter, setInteractionEndDateFilter] = useState<string>("");
  const [interactionsViewMode, setInteractionsViewMode] = useState<"kanban" | "calendar">("kanban");
  const [calendarYear, setCalendarYear] = useState<number>(2026);
  const [calendarMonth, setCalendarMonth] = useState<number>(5); // June is 5 (0-indexed)
  const [timelineFilterMode, setTimelineFilterMode] = useState<"Active" | "All">("Active");
  const [timelineZoom, setTimelineZoom] = useState<"Monthly" | "Quarterly" | "Annual">("Quarterly");

  // Process / Filter Interactions based on configuration
  const filteredInteractions = useMemo(() => {
    return interactions.filter((item) => {
      // 1. Assignee filtering
      if (interactionAssigneeFilter !== "ALL" && item.assignee !== interactionAssigneeFilter) {
        return false;
      }

      // 2. Time Range filtering
      if (interactionTimeRangeFilter !== "ALL") {
        const itemDate = new Date(item.date);
        const today = new Date();
        today.setHours(0, 0, 0, 0);

        if (interactionTimeRangeFilter === "TODAY") {
          const itemDateStr = item.date; // YYYY-MM-DD
          const todayStr = today.toISOString().split("T")[0];
          if (itemDateStr !== todayStr) return false;
        } else if (interactionTimeRangeFilter === "WEEK") {
          const sevenDaysAgo = new Date(today.getTime() - 7 * 24 * 60 * 60 * 1000);
          const sevenDaysHence = new Date(today.getTime() + 7 * 24 * 60 * 60 * 1000);
          if (itemDate < sevenDaysAgo || itemDate > sevenDaysHence) return false;
        } else if (interactionTimeRangeFilter === "MONTH") {
          const thirtyDaysAgo = new Date(today.getTime() - 30 * 24 * 60 * 60 * 1000);
          const thirtyDaysHence = new Date(today.getTime() + 30 * 24 * 60 * 60 * 1000);
          if (itemDate < thirtyDaysAgo || itemDate > thirtyDaysHence) return false;
        } else if (interactionTimeRangeFilter === "FUTURE") {
          if (itemDate < today) return false;
        } else if (interactionTimeRangeFilter === "CUSTOM") {
          if (interactionStartDateFilter) {
            const start = new Date(interactionStartDateFilter);
            start.setHours(0, 0, 0, 0);
            if (itemDate < start) return false;
          }
          if (interactionEndDateFilter) {
            const end = new Date(interactionEndDateFilter);
            end.setHours(23, 59, 59, 999);
            if (itemDate > end) return false;
          }
        }
      }

      return true;
    });
  }, [interactions, interactionAssigneeFilter, interactionTimeRangeFilter, interactionStartDateFilter, interactionEndDateFilter]);

  // Auto-clear selection lists on tab activation changes
  useEffect(() => {
    setSelectedInteractionIds([]);
    setSelectedContactIds([]);
  }, [activeTab]);

  // System Notifications
  const [toasts, setToasts] = useState<Toast[]>([]);
  const [isNotificationOpen, setIsNotificationOpen] = useState(false);
  const [activityLog, setActivityLog] = useState<string[]>([
    "Sarah K. updated status of 'Q4 Marketing Alignment' to IN PROGRESS",
    "Michael R. completed interaction log 'Enterprise Onboarding Sync'",
    "Julianne Apex was linked as stakeholder for Apex Solutions Ltd.",
    "System security credentials loaded and validated"
  ]);

  interface Notification {
    id: string;
    actionUserEmail: string;
    actionUserName: string;
    actionType: "create" | "update" | "delete";
    entityId: string;
    entityName: string;
    entityTier: string;
    message: string;
    createdAt: string;
    isRead: number;
  }

  const [notifications, setNotifications] = useState<Notification[]>([]);

  const [readVirtualNotifIds, setReadVirtualNotifIds] = useState<string[]>(() => {
    try {
      return JSON.parse(localStorage.getItem("crm_read_virtual_notifs") || "[]");
    } catch {
      return [];
    }
  });

  const markVirtualNotifAsRead = (id: string) => {
    setReadVirtualNotifIds((prev) => {
      const next = [...prev, id];
      localStorage.setItem("crm_read_virtual_notifs", JSON.stringify(next));
      return next;
    });
  };

  const markAllVirtualNotifsAsRead = (ids: string[]) => {
    setReadVirtualNotifIds((prev) => {
      const next = Array.from(new Set([...prev, ...ids]));
      localStorage.setItem("crm_read_virtual_notifs", JSON.stringify(next));
      return next;
    });
  };

  // Flag interactions marked 'SCHEDULED' or 'BLOCKED' when the due date is within 24 hours of the system date
  const impendingInteractions = useMemo(() => {
    const sysDate = new Date();
    return interactions.filter((item) => {
      if (item.status !== "SCHEDULED" && item.status !== "BLOCKED") return false;
      if (!item.date) return false;

      const targetDate = new Date(item.date);
      // item.date is typically formatted as YYYY-MM-DD
      const diffTime = targetDate.getTime() - sysDate.getTime();
      const diffHours = diffTime / (1000 * 60 * 60);

      // within 24 hours of system date (including past today/overdue and coming up tomorrow)
      return diffHours >= -24 && diffHours <= 24;
    });
  }, [interactions]);

  const virtualNotifications = useMemo(() => {
    return impendingInteractions.map((item) => {
      const id = `notif-impending-${item.id}`;
      const isRead = readVirtualNotifIds.includes(id) ? 1 : 0;
      return {
        id,
        actionUserEmail: "system@enterprise.com",
        actionUserName: "System Watchdog",
        actionType: item.status === "BLOCKED" ? "delete" as const : "update" as const,
        entityId: item.id,
        entityName: item.subject,
        entityTier: "Impending",
        message: `⏰ [${item.status}] Interaction "${item.subject}" for ${item.client} is due within 24 hours (Due: ${item.date})!`,
        createdAt: new Date(item.date).toISOString(),
        isRead
      };
    });
  }, [impendingInteractions, readVirtualNotifIds]);

  const allNotifications = useMemo(() => {
    const combined = [...virtualNotifications, ...notifications];
    return combined.sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime());
  }, [virtualNotifications, notifications]);

  interface AuditLog {
    id: string;
    userEmail: string;
    userName: string;
    userRole: string;
    actionType: "CREATE" | "UPDATE" | "DELETE" | "BATCH_DELETE" | "BATCH_UPDATE";
    targetType: "Entity" | "Contact" | "Interaction" | "Engagement" | "Note" | "Document" | "User" | "Tag";
    targetId: string;
    targetName: string;
    details: string;
    timestamp: string;
  }

  const [auditLogs, setAuditLogs] = useState<AuditLog[]>([]);

  // Filtered logs for insight calculations (either past 30 days or all-time)
  const auditTargetLogs = useMemo(() => {
    if (auditInsightRange === "30") {
      const thirtyDaysAgo = new Date();
      thirtyDaysAgo.setDate(thirtyDaysAgo.getDate() - 30);
      return auditLogs.filter(log => new Date(log.timestamp) >= thirtyDaysAgo);
    }
    return auditLogs;
  }, [auditLogs, auditInsightRange]);

  // Compute most active operators in selected timeframe
  const auditActiveOperators = useMemo(() => {
    const counts: Record<string, { email: string; name: string; role: string; count: number }> = {};
    auditTargetLogs.forEach(log => {
      const email = log.userEmail || "anonymous@enterprise.com";
      if (!counts[email]) {
        counts[email] = {
          email,
          name: log.userName || email.split("@")[0],
          role: log.userRole || "Operator",
          count: 0
        };
      }
      counts[email].count++;
    });
    return Object.values(counts).sort((a, b) => b.count - a.count).slice(0, 5);
  }, [auditTargetLogs]);

  // Compute most modified entities/records in selected timeframe
  const auditModifiedEntities = useMemo(() => {
    const counts: Record<string, {
      key: string;
      type: string;
      id: string;
      name: string;
      count: number;
      creates: number;
      updates: number;
      deletes: number;
      lastActive: string;
    }> = {};

    auditTargetLogs.forEach(log => {
      const tId = log.targetId || "unknown";
      const tType = log.targetType || "System";
      const key = `${tType}::${tId}`;
      if (!counts[key]) {
        counts[key] = {
          key,
          type: tType,
          id: tId,
          name: log.targetName || `Record #${tId.substring(0, 8)}`,
          count: 0,
          creates: 0,
          updates: 0,
          deletes: 0,
          lastActive: log.timestamp
        };
      }
      counts[key].count++;
      if (log.actionType === "CREATE") counts[key].creates++;
      else if (log.actionType === "UPDATE" || log.actionType === "BATCH_UPDATE") counts[key].updates++;
      else if (log.actionType === "DELETE" || log.actionType === "BATCH_DELETE") counts[key].deletes++;

      if (new Date(log.timestamp) > new Date(counts[key].lastActive)) {
        counts[key].lastActive = log.timestamp;
      }
    });

    return Object.values(counts).sort((a, b) => b.count - a.count).slice(0, 5);
  }, [auditTargetLogs]);

  // Compute additional metrics for summary insights
  const auditSystemMetrics = useMemo(() => {
    if (auditTargetLogs.length === 0) return { peakHour: "N/A", deletePercent: "0%", adminActionPercent: "0%" };

    // Peak hour calculation
    const hourCounts: Record<number, number> = {};
    let peakHr = 0;
    let maxCount = 0;

    // Action breakdown
    let totalActions = 0;
    let deleteActions = 0;
    let adminActions = 0;

    auditTargetLogs.forEach(log => {
      totalActions++;
      const hr = new Date(log.timestamp).getUTCHours();
      hourCounts[hr] = (hourCounts[hr] || 0) + 1;
      if (hourCounts[hr] > maxCount) {
        maxCount = hourCounts[hr];
        peakHr = hr;
      }
      if (log.actionType && log.actionType.includes("DELETE")) {
        deleteActions++;
      }
      if (log.userRole === "Senior Analyst") {
        adminActions++;
      }
    });

    const formattedPeakHour = totalActions > 0 ? `${String(peakHr).padStart(2, '0')}:00 UTC` : "N/A";
    const deletePercent = totalActions > 0 ? `${Math.round((deleteActions / totalActions) * 100)}%` : "0%";
    const adminActionPercent = totalActions > 0 ? `${Math.round((adminActions / totalActions) * 100)}%` : "0%";

    return {
      peakHour: formattedPeakHour,
      deletePercent,
      adminActionPercent
    };
  }, [auditTargetLogs]);

  // Unified memo for filtered logs used on rendering and exports
  const filteredAuditLogs = useMemo(() => {
    return auditLogs.filter((log) => {
      // Filter by dynamic vault class/target type
      if (auditTargetFilter !== "ALL" && log.targetType !== auditTargetFilter) return false;

      // Filter by specific/range actions
      if (auditActionFilter !== "ALL") {
        if (auditActionFilter === "BATCH" && (!log.actionType || !log.actionType.startsWith("BATCH"))) return false;
        if (auditActionFilter !== "BATCH" && log.actionType !== auditActionFilter) return false;
      }

      // Filter by loose query matching
      if (auditTextSearch.trim()) {
        const q = auditTextSearch.toLowerCase();
        return (
          (log.userEmail || "").toLowerCase().includes(q) ||
          (log.userName || "").toLowerCase().includes(q) ||
          (log.details || "").toLowerCase().includes(q) ||
          (log.targetName || "").toLowerCase().includes(q) ||
          (log.targetId || "").toLowerCase().includes(q) ||
          (log.userRole || "").toLowerCase().includes(q)
        );
      }
      return true;
    });
  }, [auditLogs, auditTargetFilter, auditActionFilter, auditTextSearch]);

  const handleExportAuditLogsToCSV = () => {
    if (filteredAuditLogs.length === 0) {
      showToast("No audit logs in current filter criteria to export.", "info");
      return;
    }

    // Explicit RFC-compliant CSV headers
    const headers = [
      "Timestamp (UTC)",
      "Operator Name",
      "Operator Email",
      "Operator Role",
      "Action Type",
      "Target Subsystem",
      "Target ID",
      "Target Name",
      "Transaction Details"
    ];

    // Explicit rows constructor
    const rows = filteredAuditLogs.map((log) => {
      const formattedTimestamp = new Date(log.timestamp).toISOString();
      return [
        formattedTimestamp,
        log.userName || "Anonymous",
        log.userEmail || "anonymous@enterprise.com",
        log.userRole || "Operator",
        log.actionType || "UNKNOWN",
        log.targetType || "System",
        log.targetId || "N/A",
        log.targetName || "N/A",
        log.details || ""
      ];
    });

    // Format content with double quote escaping and wrap cells holding comma, quotation marks or carriage returns
    const csvContent = [
      headers.join(","),
      ...rows.map((row) =>
        row
          .map((cell) => {
            const str = String(cell);
            const escaped = str.replace(/"/g, '""');
            if (
              escaped.includes(",") ||
              escaped.includes('"') ||
              escaped.includes("\n") ||
              escaped.includes("\r")
            ) {
              return `"${escaped}"`;
            }
            return escaped;
          })
          .join(",")
      )
    ].join("\n");

    // Standard DOM link click simulation wrapper for direct document triggers
    const blob = new Blob([csvContent], { type: "text/csv;charset=utf-8;" });
    const url = URL.createObjectURL(blob);
    const link = document.createElement("a");

    const filterDesc = [
      auditTargetFilter !== "ALL" ? `vault_${auditTargetFilter}` : "",
      auditActionFilter !== "ALL" ? `act_${auditActionFilter}` : "",
      auditTextSearch.trim() ? "search" : ""
    ]
      .filter(Boolean)
      .join("_") || "complete";

    const dateStr = new Date().toISOString().slice(0, 10);
    link.href = url;
    link.setAttribute("download", `system_audit_ledger_${filterDesc}_${dateStr}.csv`);
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
    URL.revokeObjectURL(url);

    showToast(`Successfully downloaded CSV file with ${filteredAuditLogs.length} audit records.`, "success");
  };

  const loadAuditLogs = async () => {
    if (!session || session.role !== "Senior Analyst") return;
    try {
      const res = await fetch("/api/audit-logs", {
        headers: {
          "X-User-Email": session.email,
          "X-User-Role": session.role
        }
      });
      if (res.ok) {
        const data = await res.json();
        setAuditLogs(data);
      }
    } catch (e) {
      console.error("Failed to load audit logs:", e);
    }
  };

  const purgeAuditLogs = async () => {
    if (!session || session.role !== "Senior Analyst") return;
    if (!window.confirm("CRITICAL WARNING: Are you absolutely sure you want to permanently purge all system audit logs? This is irreversible and will lose full lineage visibility.")) {
      return;
    }
    try {
      const res = await fetch("/api/audit-logs/purge", {
        method: "POST",
        headers: {
          "X-User-Email": session.email,
          "X-User-Role": session.role
        }
      });
      if (res.ok) {
        showToast("Audit ledger purged successfully.", "info");
        loadAuditLogs();
      }
    } catch (e) {
      console.error("Failed to purge audit logs:", e);
    }
  };

  // Hashing Function Fallback for browser sandboxing
  const hashPassword = async (pwd: string): Promise<string> => {
    let hash = 0;
    const salt = "_crm_salt_2026_";
    const salted = pwd + salt;
    for (let i = 0; i < salted.length; i++) {
      hash = (hash << 5) - hash + salted.charCodeAt(i);
      hash = hash & hash;
    }
    return Math.abs(hash).toString(16);
  };

  // SQLite API synchronization helper
  const syncToServer = async (url: string, method: string, body?: any) => {
    try {
      const headers: Record<string, string> = { "Content-Type": "application/json" };
      if (session) {
        headers["X-User-Email"] = session.email;
        headers["X-User-Name"] = session.name;
        headers["X-User-Role"] = session.role;
      }
      const res = await fetch(url, {
        method,
        headers,
        body: body ? JSON.stringify(body) : undefined
      });
      if (!res.ok) {
        console.error("SQLite Sync Error:", await res.text());
      }
    } catch (e) {
      console.error("Failed to sync SQLite backend:", e);
    }
  };

  // Pull loadSQLiteState out of useEffect so we can call it on demand (e.g., when WebSocket notification arrives)
  const loadSQLiteState = async () => {
    try {
      const res = await fetch("/api/all");
      if (res.ok) {
        const data = await res.json();
        if (data.tags) setTags(data.tags);
        if (data.notes) setNotes(data.notes);
        if (data.documents) setDocuments(data.documents);
        if (data.interactions) setInteractions(data.interactions);
        if (data.contacts) setContacts(data.contacts);
        if (data.entities) setEntities(data.entities);
        if (data.engagements) setEngagements(data.engagements);
        if (data.systemUsers) {
          setSystemUsers(data.systemUsers);
          localStorage.setItem("crm_users", JSON.stringify(data.systemUsers));
        }
      }
    } catch (err) {
      console.error("Failed to load SQLite data:", err);
    }
  };

  const loadNotifications = async () => {
    if (!session) return;
    try {
      const res = await fetch("/api/notifications", {
        headers: {
          "X-User-Email": session.email,
          "X-User-Role": session.role
        }
      });
      if (res.ok) {
        const data = await res.json();
        setNotifications(data);
      }
    } catch (e) {
      console.error("Failed to load notifications:", e);
    }
  };

  const markNotificationAsRead = async (id: string) => {
    if (!session) return;
    try {
      const res = await fetch("/api/notifications/read", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "X-User-Email": session.email,
          "X-User-Role": session.role
        },
        body: JSON.stringify({ notificationIds: [id] })
      });
      if (res.ok) {
        setNotifications((prev) =>
          prev.map((n) => (n.id === id ? { ...n, isRead: 1 } : n))
        );
      }
    } catch (e) {
      console.error("Failed to mark notification as read:", e);
    }
  };

  const markAllNotificationsAsRead = async () => {
    if (!session) return;
    try {
      const res = await fetch("/api/notifications/read-all", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "X-User-Email": session.email,
          "X-User-Role": session.role
        }
      });
      if (res.ok) {
        setNotifications((prev) => prev.map((n) => ({ ...n, isRead: 1 })));
      }
    } catch (e) {
      console.error("Failed to mark all notifications as read:", e);
    }
  };

  // Load all initial state from backend SQLite DB on mount
  useEffect(() => {
    loadSQLiteState();
  }, []);

  // Sync notes to local storage
  useEffect(() => {
    localStorage.setItem("crm_notes", JSON.stringify(notes));
  }, [notes]);

  // Fetch notifications on mount & session changes
  useEffect(() => {
    loadNotifications();
  }, [session]);

  // Fetch audit logs on mount & session changes or when audit tab is open
  useEffect(() => {
    if (session && session.role === "Senior Analyst") {
      loadAuditLogs();
    }
  }, [session, activeTab]);

  // WebSocket connection for real-time notifications
  useEffect(() => {
    if (!session) return;

    const protocol = window.location.protocol === "https:" ? "wss:" : "ws:";
    const wsUrl = `${protocol}//${window.location.host}/ws?email=${encodeURIComponent(session.email)}&role=${encodeURIComponent(session.role)}`;

    let ws: WebSocket;
    let reconnectTimer: any;

    const connect = () => {
      ws = new WebSocket(wsUrl);

      ws.onopen = () => {
        console.log("WebSocket connection established");
      };

      ws.onmessage = (event) => {
        try {
          const msg = JSON.parse(event.data);
          if (msg.type === "NOTIFICATION_RECEIVED") {
            setNotifications((prev) => [msg.data, ...prev]);
            showToast(`🔔 ${msg.data.message}`, "info");
            loadSQLiteState();
          }
        } catch (err) {
          console.error("Failed to parse websocket message:", err);
        }
      };

      ws.onclose = () => {
        console.log("WebSocket closed, schedule reconnect...");
        reconnectTimer = setTimeout(() => {
          connect();
        }, 5000);
      };

      ws.onerror = (err) => {
        console.error("WebSocket encountered error:", err);
        ws.close();
      };
    };

    connect();

    return () => {
      if (ws) {
        ws.onclose = () => {};
        ws.close();
      }
      clearTimeout(reconnectTimer);
    };
  }, [session]);

  const showToast = (message: string, type: Toast["type"] = "success") => {
    const newToast: Toast = { id: Math.random().toString(), message, type };
    setToasts((prev) => [...prev, newToast]);
    setActivityLog((prev) => [`[${new Date().toLocaleTimeString()}] ${message}`, ...prev.slice(0, 15)]);
    setTimeout(() => { setToasts((prev) => prev.filter((t) => t.id !== newToast.id)); }, 4000);
  };

  // --- CLIENT-SIDE BATCH ACTION HANDLERS ---
  const handleBatchInteractionDelete = async () => {
    if (selectedInteractionIds.length === 0) return;
    if (!window.confirm(`Are you sure you want to delete ${selectedInteractionIds.length} selected interactions?`)) return;

    try {
      const res = await fetch("/api/interactions/batch-delete", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ ids: selectedInteractionIds })
      });
      if (res.ok) {
        setInteractions(prev => prev.filter(i => !selectedInteractionIds.includes(i.id)));
        showToast(`Successfully deleted ${selectedInteractionIds.length} interactions`, "success");
        setSelectedInteractionIds([]);
      } else {
        showToast("Failed to perform batch deletion", "error");
      }
    } catch (err) {
      console.error(err);
      showToast("Error executing batch deletion", "error");
    }
  };

  const handleBatchInteractionStatusUpdate = async (newStatus: "IN PROGRESS" | "SCHEDULED" | "COMPLETED" | "BLOCKED") => {
    if (selectedInteractionIds.length === 0) return;
    try {
      const res = await fetch("/api/interactions/batch-update-status", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ ids: selectedInteractionIds, status: newStatus })
      });
      if (res.ok) {
        setInteractions(prev => prev.map(i => {
          if (selectedInteractionIds.includes(i.id)) {
            return { ...i, status: newStatus };
          }
          return i;
        }));
        showToast(`Updated ${selectedInteractionIds.length} interactions to ${newStatus}`, "success");
        setSelectedInteractionIds([]);
      } else {
        showToast("Failed to update status in batch", "error");
      }
    } catch (err) {
      console.error(err);
      showToast("Error updating batch statuses", "error");
    }
  };

  const handleBatchContactDelete = async () => {
    if (selectedContactIds.length === 0) return;
    if (!window.confirm(`Are you sure you want to delete ${selectedContactIds.length} selected contacts?`)) return;

    try {
      const res = await fetch("/api/contacts/batch-delete", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ ids: selectedContactIds })
      });
      if (res.ok) {
        setContacts(prev => prev.filter(c => !selectedContactIds.includes(c.id)));
        showToast(`Successfully deleted ${selectedContactIds.length} contacts`, "success");
        setSelectedContactIds([]);
      } else {
        showToast("Failed to perform batch deletion", "error");
      }
    } catch (err) {
      console.error(err);
      showToast("Error executing batch deletion", "error");
    }
  };

  const handleBatchContactRoleUpdate = async (newRole: string) => {
    if (selectedContactIds.length === 0 || !newRole) return;
    try {
      const res = await fetch("/api/contacts/batch-update-role", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ ids: selectedContactIds, role: newRole })
      });
      if (res.ok) {
        setContacts(prev => prev.map(c => {
          if (selectedContactIds.includes(c.id)) {
            return { ...c, role: newRole };
          }
          return c;
        }));
        showToast(`Updated role of ${selectedContactIds.length} contacts to ${newRole}`, "success");
        setSelectedContactIds([]);
      } else {
        showToast("Failed to update contact roles in batch", "error");
      }
    } catch (err) {
      console.error(err);
      showToast("Error updating batch roles", "error");
    }
  };

  const handleBatchContactCompanyUpdate = async (newCompany: string) => {
    if (selectedContactIds.length === 0 || !newCompany) return;
    try {
      const res = await fetch("/api/contacts/batch-update-company", {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ ids: selectedContactIds, company: newCompany })
      });
      if (res.ok) {
        setContacts(prev => prev.map(c => {
          if (selectedContactIds.includes(c.id)) {
            return { ...c, company: newCompany };
          }
          return c;
        }));
        showToast(`Changed company of ${selectedContactIds.length} contacts to ${newCompany}`, "success");
        setSelectedContactIds([]);
      } else {
        showToast("Failed to update contact company in batch", "error");
      }
    } catch (err) {
      console.error(err);
      showToast("Error updating batch company details", "error");
    }
  };


  // Core Authentication Routines
  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!authEmail || !authPassword) {
      showToast("Please provide both email and passcode.", "error");
      return;
    }
    const users = JSON.parse(localStorage.getItem("crm_users") || "[]");
    const user = users.find((u: any) => u.email.toLowerCase() === authEmail.toLowerCase());
    if (!user) {
      showToast("Access denined. Unrecognised operator unit.", "error");
      return;
    }
    if (user.suspended) {
      showToast("Security Exception: This operator seat has been suspended by system administrator.", "error");
      return;
    }
    const hashed = await hashPassword(authPassword);
    if (user.passwordHash !== hashed) {
      showToast("Invalid credentials hash match failed.", "error");
      return;
    }
    const sessionData = { email: user.email, name: user.name, role: user.role };
    setSession(sessionData);
    localStorage.setItem("crm_active_session", JSON.stringify(sessionData));
    showToast(`Welcome back, Operator ${user.name}! Connected.`, "success");
  };

  const handleSignup = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!authEmail || !authPassword || !authConfirmPassword || !authName) {
      showToast("Please fully complete registration credentials.", "error");
      return;
    }
    if (authPassword !== authConfirmPassword) {
      showToast("Confirmation coordinates mismatch.", "error");
      return;
    }
    const users = JSON.parse(localStorage.getItem("crm_users") || "[]");
    if (users.some((u: any) => u.email.toLowerCase() === authEmail.toLowerCase())) {
      showToast("This email has already been registered.", "error");
      return;
    }
    const hashed = await hashPassword(authPassword);
    const newUser = { email: authEmail, passwordHash: hashed, name: authName, role: authRole };
    const updatedUsers = [...users, newUser];
    localStorage.setItem("crm_users", JSON.stringify(updatedUsers));
    setSystemUsers(updatedUsers);
    setSession({ email: authEmail, name: authName, role: authRole });
    localStorage.setItem("crm_active_session", JSON.stringify({ email: authEmail, name: authName, role: authRole }));
    showToast(`Operator profiles set up successfully. Connected.`, "success");
  };

  const handleResetPassword = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!authEmail || !authName || !authPassword || !authConfirmPassword) {
      showToast("Please provide matching identifiers to update password.", "error");
      return;
    }
    if (authPassword !== authConfirmPassword) {
      showToast("Credential verification mismatch.", "error");
      return;
    }
    const users = JSON.parse(localStorage.getItem("crm_users") || "[]");
    const idx = users.findIndex((u: any) => u.email.toLowerCase() === authEmail.toLowerCase() && u.name.toLowerCase() === authName.toLowerCase());
    if (idx === -1) {
      showToast("Operator identifier verification failed.", "error");
      return;
    }
    users[idx].passwordHash = await hashPassword(authPassword);
    localStorage.setItem("crm_users", JSON.stringify(users));
    setSystemUsers(users);
    showToast("Operator passphrase securely updated. Please login.", "success");
    setAuthScreen("login");
  };

  const handleLogout = () => {
    setSession(null);
    localStorage.removeItem("crm_active_session");
    showToast("Operator session closed. Local matrix locked.", "info");
  };

  // Permission-aware Entities Visible to Active Session User
  const visibleEntities = useMemo(() => {
    if (!session) return [];
    return entities.filter((e) => {
      // Senior Analyst gets all access
      if (session.role === "Senior Analyst") return true;
      // All other roles do not have permission to view Strategic accounts
      if (e.tier === "Strategic") return false;
      return true;
    });
  }, [entities, session]);

  // Search filter index query matching
  const searchResults = useMemo(() => {
    if (!searchQuery) return { interactions: [], contacts: [], entities: [], engagements: [] };
    const q = searchQuery.toLowerCase();
    return {
      interactions: interactions.filter((i) => (i.subject || "").toLowerCase().includes(q) || (i.summary || "").toLowerCase().includes(q) || (i.client || "").toLowerCase().includes(q)),
      contacts: contacts.filter((c) => (c.name || "").toLowerCase().includes(q) || (c.role || "").toLowerCase().includes(q) || (c.company || "").toLowerCase().includes(q) || (c.email || "").toLowerCase().includes(q)),
      entities: visibleEntities.filter((e) => (e.name || "").toLowerCase().includes(q) || (e.industry || "").toLowerCase().includes(q) || (e.location || "").toLowerCase().includes(q)),
      engagements: engagements.filter((g) => (g.title || "").toLowerCase().includes(g.title ? q : "") || (g.client || "").toLowerCase().includes(g.client ? q : "") || (g.type || "").toLowerCase().includes(g.type ? q : "") || (g.description || "").toLowerCase().includes(g.description ? q : "")),
      total: 0
    };
  }, [searchQuery, interactions, contacts, visibleEntities, engagements]);

  useEffect(() => {
    setIsSearchActive(searchQuery.trim().length > 0);
  }, [searchQuery]);

  // CRUD actions
  const handleCreateEntry = async (e: React.FormEvent) => {
    e.preventDefault();
    if (newType === "interaction") {
      if (!intForm.subject || !intForm.client) {
        showToast("Subject and Corporate Account link required.", "error");
        return;
      }
      const newInt: Interaction = {
        id: `int-${Date.now()}`,
        ...intForm,
        tagIds: []
      };
      setInteractions([newInt, ...interactions]);
      syncToServer("/api/interactions", "POST", newInt);
      showToast(`Meeting interaction '${intForm.subject}' created.`, "success");
      setIntForm({ subject: "", type: "Meeting", assignee: ASSIGNEES[0], status: "IN PROGRESS", client: "", date: new Date().toISOString().split("T")[0], summary: "" });
    } else if (newType === "contact") {
      if (!contactForm.name || !contactForm.company || !contactForm.email) {
        showToast("Contact Name, email, and corporate company required.", "error");
        return;
      }
      const newCon: Contact = { id: `con-${Date.now()}`, ...contactForm };
      setContacts([newCon, ...contacts]);
      syncToServer("/api/contacts", "POST", newCon);
      showToast(`Client contact '${contactForm.name}' indexed.`, "success");
      setContactForm({ name: "", role: "", company: "", email: "", phone: "" });
    } else if (newType === "entity") {
      if (!entityForm.name || !entityForm.industry) {
        showToast("Corporate Organization Name and industry classification required.", "error");
        return;
      }
      const newEnt: Entity = { id: `ent-${Date.now()}`, ...entityForm };
      setEntities([newEnt, ...entities]);
      syncToServer("/api/entities", "POST", newEnt);
      showToast(`Corporate Account '${entityForm.name}' established.`, "success");
      setEntityForm({ name: "", industry: "", tier: "Enterprise", location: "" });
    } else if (newType === "engagement") {
      if (!engagementForm.title || !engagementForm.client) {
        showToast("Engagement Title and Associated Client are required.", "error");
        return;
      }
      const newEng: Engagement = {
        id: `eng-${Date.now()}`,
        ...engagementForm
      };
      setEngagements([newEng, ...engagements]);
      syncToServer("/api/engagements", "POST", newEng);
      showToast(`Engagement Initiative '${engagementForm.title}' established.`, "success");
      setEngagementForm({ title: "", client: "", type: "SOW Contract", budget: "$", startDate: new Date().toISOString().split("T")[0], endDate: new Date().toISOString().split("T")[0], status: "Active", description: "" });
    } else if (newType === "user") {
      if (!operatorForm.name || !operatorForm.email || !operatorForm.passphrase) {
        showToast("operator Name, email, and passphrase required.", "error");
        return;
      }
      const hashed = await hashPassword(operatorForm.passphrase);
      const newUser = { email: operatorForm.email, passwordHash: hashed, name: operatorForm.name, role: operatorForm.role };
      setSystemUsers([...systemUsers, newUser]);
      syncToServer("/api/users", "POST", newUser);
      showToast(`Operator profile registered: ${operatorForm.name}`, "success");
      setOperatorForm({ name: "", email: "", role: "Senior Analyst", passphrase: "" });
    }
    setIsNewModalOpen(false);
  };

  const handleUpdateItem = (type: "interaction" | "contact" | "entity" | "engagement" | "user", payload: any) => {
    if (type === "interaction") {
      setInteractions((prev) => prev.map((item) => (item.id === payload.id ? payload : item)));
      syncToServer(`/api/interactions/${payload.id}`, "PUT", payload);
      showToast("Interaction briefing updated.", "success");
    } else if (type === "contact") {
      setContacts((prev) => prev.map((item) => (item.id === payload.id ? payload : item)));
      syncToServer(`/api/contacts/${payload.id}`, "PUT", payload);
      showToast("Contact coordinates updated.", "success");
    } else if (type === "entity") {
      setEntities((prev) => prev.map((item) => (item.id === payload.id ? payload : item)));
      syncToServer(`/api/entities/${payload.id}`, "PUT", payload);
      showToast("Corporate account profiles modified.", "success");
    } else if (type === "engagement") {
      setEngagements((prev) => prev.map((item) => (item.id === payload.id ? payload : item)));
      syncToServer(`/api/engagements/${payload.id}`, "PUT", payload);
      showToast("Engagement details modified.", "success");
    } else if (type === "user") {
      setSystemUsers((prev) => prev.map((item) => (item.email === payload.email ? payload : item)));
      syncToServer(`/api/users`, "PUT", payload);
      showToast("Operator Seat updated successfully.", "success");
    }
    setSelectedItem(null);
  };

  const handleDeleteItem = (type: "interaction" | "contact" | "entity" | "engagement" | "user", id: string) => {
    if (type === "interaction") {
      setInteractions((prev) => prev.filter((item) => item.id !== id));
      syncToServer(`/api/interactions/${id}`, "DELETE");
      showToast("Interaction registry removed.", "info");
    } else if (type === "contact") {
      setContacts((prev) => prev.filter((item) => item.id !== id));
      syncToServer(`/api/contacts/${id}`, "DELETE");
      showToast("Stakeholder contact card removed.", "info");
    } else if (type === "entity") {
      setEntities((prev) => prev.filter((item) => item.id !== id));
      syncToServer(`/api/entities/${id}`, "DELETE");
      showToast("Corporate profile structure discarded.", "info");
    } else if (type === "engagement") {
      setEngagements((prev) => prev.filter((item) => item.id !== id));
      syncToServer(`/api/engagements/${id}`, "DELETE");
      showToast("Engagement Initiative discarded.", "info");
    } else if (type === "user") {
      if (id.toLowerCase() === session?.email.toLowerCase()) {
        showToast("Constraint Violation: You cannot suspend your own active operator seat session.", "error");
        return;
      }
      const activeUsers = systemUsers.filter((u) => !u.suspended);
      if (activeUsers.length <= 1) {
        showToast("Constraint Violation: Workspace must retain at least 1 active operator seat.", "error");
        return;
      }
      const activeSeniorAnalysts = systemUsers.filter((u) => u.role === "Senior Analyst" && !u.suspended);
      const targetUser = systemUsers.find((u) => u.email === id);
      if (targetUser && targetUser.role === "Senior Analyst" && activeSeniorAnalysts.length <= 1) {
        showToast("Constraint Violation: Workspace must retain at least 1 active Primary Seat (Senior Analyst) Operator.", "error");
        return;
      }

      setSystemUsers((prev) =>
        prev.map((item) => (item.email === id ? { ...item, suspended: 1 } : item))
      );
      syncToServer(`/api/users/${id}`, "DELETE");
      showToast("Operator profile suspended.", "info");
    }
    setSelectedItem(null);
  };

  const handleRestoreUser = async (email: string) => {
    try {
      const targetUser = systemUsers.find((u) => u.email === email);
      if (targetUser) {
        setSystemUsers((prev) =>
          prev.map((item) => (item.email === email ? { ...item, suspended: 0 } : item))
        );
        const res = await fetch(`/api/users/${email}/restore`, { method: "POST" });
        if (res.ok) {
          showToast(`Seat privileges restored successfully for ${targetUser.name}.`, "success");
          loadSQLiteState();
        } else {
          showToast("Failed to restore operator seat privileges.", "error");
        }
      }
    } catch (e) {
      console.error(e);
      showToast("Failed to restore operator seat.", "error");
    }
  };

  // Dynamic Connection Sync Helpers inside open drawer
  const linkedTags = useMemo(() => {
    if (!selectedItem) return [];
    if (selectedItem.dataType === "interaction") {
      return tags.filter((t) => selectedItem.data.tagIds?.includes(t.id));
    }
    // For other datatypes, we can filter tags implicitly linked or support flexible tag association
    return [];
  }, [selectedItem, tags]);

  const linkedNotes = useMemo(() => {
    if (!selectedItem) return [];
    return notes
      .filter((n) => n.linkedToType === selectedItem.dataType && n.linkedToId === selectedItem.data.id)
      .sort((a, b) => {
        const aPinned = a.pinned ? 1 : 0;
        const bPinned = b.pinned ? 1 : 0;
        if (aPinned !== bPinned) return bPinned - aPinned;
        return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
      });
  }, [selectedItem, notes]);

  const filteredAndSortedNotes = useMemo(() => {
    const q = searchQuery.toLowerCase().trim();
    const filtered = notes.filter((note) => {
      if (!q) return true;
      return (
        note.content.toLowerCase().includes(q) ||
        note.author.toLowerCase().includes(q)
      );
    });
    return filtered.sort((a, b) => {
      const aPinned = a.pinned ? 1 : 0;
      const bPinned = b.pinned ? 1 : 0;
      if (aPinned !== bPinned) return bPinned - aPinned;
      return new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime();
    });
  }, [notes, searchQuery]);

  const linkedDocs = useMemo(() => {
    if (!selectedItem) return [];
    return documents.filter((d) => d.linkedToType === selectedItem.dataType && d.linkedToId === selectedItem.data.id);
  }, [selectedItem, documents]);

  // Options to link unassigned elements
  const availableUnlinkedTags = useMemo(() => {
    if (!selectedItem || selectedItem.dataType !== "interaction") return [];
    const used = selectedItem.data.tagIds || [];
    return tags.filter((t) => !used.includes(t.id));
  }, [selectedItem, tags]);

  const handleLinkTag = () => {
    if (!selectedTagToLink || !selectedItem || selectedItem.dataType !== "interaction") return;
    const updated = {
      ...selectedItem.data,
      tagIds: [...(selectedItem.data.tagIds || []), selectedTagToLink]
    };
    setInteractions((prev) => prev.map((i) => (i.id === updated.id ? updated : i)));
    setSelectedItem({ ...selectedItem, data: updated });
    setSelectedTagToLink("");
    showToast("Workspace tag connection mapped.", "success");
    syncToServer(`/api/interactions/${updated.id}`, "PUT", updated);
  };

  const handleUnlinkTag = (tagId: string) => {
    if (!selectedItem || selectedItem.dataType !== "interaction") return;
    const updated = {
      ...selectedItem.data,
      tagIds: (selectedItem.data.tagIds || []).filter((id: string) => id !== tagId)
    };
    setInteractions((prev) => prev.map((i) => (i.id === updated.id ? updated : i)));
    setSelectedItem({ ...selectedItem, data: updated });
    showToast("Workspace tag connection unlinked.", "info");
    syncToServer(`/api/interactions/${updated.id}`, "PUT", updated);
  };

  const handleCreateAndLinkTag = () => {
    if (!newTagName.trim() || !selectedItem || selectedItem.dataType !== "interaction") return;
    const tagId = `tag-${Date.now()}`;
    const newTag: CustomTag = { id: tagId, name: newTagName.trim(), color: newTagColor };
    setTags((prev) => [...prev, newTag]);
    syncToServer("/api/tags", "POST", newTag);

    const updated = {
      ...selectedItem.data,
      tagIds: [...(selectedItem.data.tagIds || []), tagId]
    };
    setInteractions((prev) => prev.map((i) => (i.id === updated.id ? updated : i)));
    setSelectedItem({ ...selectedItem, data: updated });
    setNewTagName("");
    showToast(`New tag #${newTagName} established and linked.`, "success");
    syncToServer(`/api/interactions/${updated.id}`, "PUT", updated);
  };

  const handleAddNote = () => {
    if (!newNoteContent.trim() || !selectedItem) return;
    const newNote: Note = {
      id: `note-${Date.now()}`,
      content: newNoteContent.trim(),
      createdAt: new Date().toISOString(),
      author: session?.name || "David J.",
      linkedToType: selectedItem.dataType,
      linkedToId: selectedItem.data.id
    };
    setNotes((prev) => [...prev, newNote]);
    setNewNoteContent("");
    showToast("Linked activity log note added.", "success");
    syncToServer("/api/notes", "POST", newNote);
  };

  const handleDeleteNote = (noteId: string) => {
    setNotes((prev) => prev.filter((n) => n.id !== noteId));
    showToast("Linked note log discarded.", "info");
    syncToServer(`/api/notes/${noteId}`, "DELETE");
  };

  const handleTogglePinNote = (noteId: string) => {
    let targetNote: Note | undefined;
    setNotes((prev) =>
      prev.map((n) => {
        if (n.id === noteId) {
          const updated = { ...n, pinned: n.pinned ? 0 : 1 };
          targetNote = updated;
          return updated;
        }
        return n;
      })
    );
    // Use a small timeout or wait to sync
    setTimeout(() => {
      // Find the updated note from the setNotes update, or construct it
      const currentNotes = JSON.parse(localStorage.getItem("crm_notes") || "[]");
      const found = currentNotes.find((n: Note) => n.id === noteId);
      if (found) {
        syncToServer("/api/notes", "POST", found);
        showToast(found.pinned ? "Ledger note pinned to top." : "Ledger note unpinned.", "success");
      }
    }, 100);
  };

  const handleAddDocument = () => {
    if (!newDocTitle.trim() || !selectedItem) {
      showToast("Please provide document file name.", "error");
      return;
    }
    const newDoc: Document = {
      id: `doc-${Date.now()}`,
      title: newDocTitle.trim(),
      fileType: newDocType,
      fileSize: newDocSize.trim() || "1.4 MB",
      uploadedAt: new Date().toISOString(),
      linkedToType: selectedItem.dataType as any,
      linkedToId: selectedItem.data.id
    };
    setDocuments((prev) => [...prev, newDoc]);
    setNewDocTitle("");
    setNewDocSize("");
    showToast("Corporate document attachment linked in database.", "success");
    syncToServer("/api/documents", "POST", newDoc);
  };

  const handleDeleteDocument = (docId: string) => {
    setDocuments((prev) => prev.filter((d) => d.id !== docId));
    showToast("Document reference attachment removed.", "info");
    syncToServer(`/api/documents/${docId}`, "DELETE");
  };

  const getColorClasses = (color: CustomTag["color"]) => {
    switch (color) {
      case "red": return "bg-rose-50 border-rose-200 text-rose-700";
      case "amber": return "bg-amber-50 border-amber-200 text-amber-700";
      case "emerald": return "bg-emerald-50 border-emerald-200 text-emerald-700";
      case "purple": return "bg-purple-50 border-purple-200 text-purple-700";
      case "indigo": return "bg-indigo-50 border-indigo-200 text-indigo-700";
      default: return "bg-blue-50 border-blue-200 text-blue-700";
    }
  };

  const getStatusClasses = (status: Interaction["status"]) => {
    switch (status) {
      case "COMPLETED":
        return {
          bg: "bg-emerald-50 border-emerald-200 text-emerald-700",
          dot: "bg-emerald-500",
          border: "border-l-4 border-l-emerald-500",
          text: "COMPLETED"
        };
      case "BLOCKED":
        return {
          bg: "bg-rose-50 border-rose-200 text-rose-700",
          dot: "bg-rose-500",
          border: "border-l-4 border-l-rose-500",
          text: "BLOCKED"
        };
      case "IN PROGRESS":
        return {
          bg: "bg-amber-50 border-amber-200 text-amber-500",
          dot: "bg-amber-500",
          border: "border-l-4 border-l-amber-500",
          text: "IN PROGRESS"
        };
      case "SCHEDULED":
        return {
          bg: "bg-blue-50 border-blue-200 text-blue-600",
          dot: "bg-blue-500",
          border: "border-l-4 border-l-blue-500",
          text: "SCHEDULED"
        };
      default:
        return {
          bg: "bg-slate-50 border-slate-200 text-slate-500",
          dot: "bg-slate-400",
          border: "border-l-4 border-l-slate-400",
          text: status
        };
    }
  };

  // Secure Auth Console Intercept
  if (!session) {
    return (
      <div id="secure-auth-workspace" className="min-h-screen bg-slate-950 flex flex-col justify-center items-center p-4 relative overflow-hidden font-sans text-slate-300">
        <div className="absolute top-0 left-0 w-96 h-96 bg-blue-600/10 rounded-full blur-[100px] pointer-events-none" />
        <div className="absolute bottom-0 right-0 w-96 h-96 bg-purple-600/10 rounded-full blur-[100px] pointer-events-none" />

        <div className="w-full max-w-md bg-slate-900/60 border border-slate-800 backdrop-blur-xl rounded-2xl shadow-2xl p-8 space-y-6 relative z-10">
          <div className="text-center space-y-2">
            <div className="inline-flex w-12 h-12 bg-blue-600 rounded-xl items-center justify-center text-white font-extrabold text-xl shadow-[0_0_20px_rgba(37,99,235,0.3)]">
              E
            </div>
            <h2 className="text-xl font-bold tracking-tight text-white">Enterprise Alignment Hub</h2>
            <p className="text-xs text-slate-500">Security-authenticated interaction and document link portal</p>
          </div>

          {authScreen === "login" && (
            <form onSubmit={handleLogin} className="space-y-4">
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Operator Coordinates (Email)</label>
                <div className="relative">
                  <input
                    type="email"
                    required
                    placeholder="david@enterprise.com"
                    value={authEmail}
                    onChange={(e) => setAuthEmail(e.target.value)}
                    className="w-full bg-slate-950 border border-slate-800 p-2.5 pl-10 rounded-lg text-white placeholder-slate-600 focus:border-blue-500 outline-none text-xs transition"
                  />
                  <Mail className="w-4 h-4 text-slate-600 absolute left-3.5 top-3.5" />
                </div>
              </div>

              <div className="space-y-1">
                <div className="flex justify-between items-center">
                  <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Passphrase</label>
                  <button type="button" onClick={() => setAuthScreen("reset")} className="text-[10px] text-blue-500 hover:underline">Reset Token</button>
                </div>
                <div className="relative">
                  <input
                    type={showPassword ? "text" : "password"}
                    required
                    placeholder="••••••••"
                    value={authPassword}
                    onChange={(e) => setAuthPassword(e.target.value)}
                    className="w-full bg-slate-950 border border-slate-800 p-2.5 pl-10 pr-10 rounded-lg text-white placeholder-slate-600 focus:border-blue-500 outline-none text-xs transition"
                  />
                  <Lock className="w-4 h-4 text-slate-600 absolute left-3.5 top-3.5" />
                  <button type="button" onClick={() => setShowPassword(!showPassword)} className="absolute right-3.5 top-3.5 text-slate-500">
                    {showPassword ? <EyeOff className="w-4 h-4" /> : <Eye className="w-4 h-4" />}
                  </button>
                </div>
              </div>

              <button type="submit" className="w-full py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-bold tracking-wide shadow-lg flex justify-center items-center gap-2">
                <ShieldCheck className="w-4 h-4" /> Authenticate Operator
              </button>

              <div className="pt-2 border-t border-slate-800 text-center text-[11px] text-slate-500">
                New user? <button type="button" onClick={() => setAuthScreen("signup")} className="text-blue-500 font-bold hover:underline">Register Profile</button>
              </div>

              <div className="p-3 bg-slate-950/40 border border-slate-800/40 rounded-lg text-center font-mono text-[9px] text-slate-500">
                <span className="font-bold text-slate-400">Pre-Seeded Credentials:</span>
                <p>david@enterprise.com | password123</p>
              </div>
            </form>
          )}

          {authScreen === "signup" && (
            <form onSubmit={handleSignup} className="space-y-4">
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Full Name</label>
                <input
                  type="text"
                  required
                  placeholder="e.g. Julianne Apex"
                  value={authName}
                  onChange={(e) => setAuthName(e.target.value)}
                  className="w-full bg-slate-950 border border-slate-800 p-2.5 rounded-lg text-white text-xs outline-none"
                />
              </div>
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Email</label>
                <input
                  type="email"
                  required
                  placeholder="name@company.com"
                  value={authEmail}
                  onChange={(e) => setAuthEmail(e.target.value)}
                  className="w-full bg-slate-950 border border-slate-800 p-2.5 rounded-lg text-white text-xs outline-none"
                />
              </div>
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Passphrase</label>
                <input
                  type="password"
                  required
                  placeholder="Min 6 characters"
                  value={authPassword}
                  onChange={(e) => setAuthPassword(e.target.value)}
                  className="w-full bg-slate-950 border border-slate-800 p-2.5 rounded-lg text-white text-xs outline-none"
                />
              </div>
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Confirm Passphrase</label>
                <input
                  type="password"
                  required
                  placeholder="Confirm passphrase"
                  value={authConfirmPassword}
                  onChange={(e) => setAuthConfirmPassword(e.target.value)}
                  className="w-full bg-slate-950 border border-slate-800 p-2.5 rounded-lg text-white text-xs outline-none"
                />
              </div>

              <button type="submit" className="w-full py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-bold tracking-wide">
                Establish Corporate Operator Profile
              </button>
              <div className="text-center text-[11px] text-slate-500">
                Back to <button type="button" onClick={() => setAuthScreen("login")} className="text-blue-500 hover:underline">Access Console</button>
              </div>
            </form>
          )}

          {authScreen === "reset" && (
            <form onSubmit={handleResetPassword} className="space-y-4">
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Registered Email</label>
                <input
                  type="email"
                  required
                  placeholder="david@enterprise.com"
                  value={authEmail}
                  onChange={(e) => setAuthEmail(e.target.value)}
                  className="w-full bg-slate-950 border border-slate-800 p-2.5 rounded-lg text-white text-xs outline-none"
                />
              </div>
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Operator Name (Exact Match)</label>
                <input
                  type="text"
                  required
                  placeholder="David Jenkins"
                  value={authName}
                  onChange={(e) => setAuthName(e.target.value)}
                  className="w-full bg-slate-950 border border-slate-800 p-2.5 rounded-lg text-white text-xs outline-none"
                />
              </div>
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">New Password</label>
                <input
                  type="password"
                  required
                  placeholder="••••••••"
                  value={authPassword}
                  onChange={(e) => setAuthPassword(e.target.value)}
                  className="w-full bg-slate-950 border border-slate-800 p-2.5 rounded-lg text-white text-xs outline-none"
                />
              </div>
              <div className="space-y-1">
                <label className="block text-[10px] uppercase font-bold text-slate-500 tracking-wider">Confirm New Password</label>
                <input
                  type="password"
                  required
                  placeholder="••••••••"
                  value={authConfirmPassword}
                  onChange={(e) => setAuthConfirmPassword(e.target.value)}
                  className="w-full bg-slate-950 border border-slate-800 p-2.5 rounded-lg text-white text-xs outline-none"
                />
              </div>

              <button type="submit" className="w-full py-2.5 bg-blue-600 hover:bg-blue-700 text-white rounded-lg text-xs font-bold">
                Reset Access Token
              </button>
              <div className="text-center text-[11px] text-slate-500">
                Cancel reset and <button type="button" onClick={() => setAuthScreen("login")} className="text-blue-500 hover:underline">Go Back</button>
              </div>
            </form>
          )}
        </div>
      </div>
    );
  }

  return (
    <div id="enterprise-crm-workspace" className="min-h-screen bg-slate-50 flex text-slate-800 antialiased font-sans">

      {/* Dynamic Notifications Toasts */}
      <div className="fixed top-4 right-4 z-[100] flex flex-col gap-2 max-w-sm pointer-events-none">
        {toasts.map((t) => (
          <div
            key={t.id}
            className={`pointer-events-auto p-4 rounded-xl shadow-xl flex items-center justify-between gap-3 border transition-all duration-300 ${
              t.type === "success"
                ? "bg-white border-emerald-100 text-slate-900"
                : t.type === "error"
                ? "bg-rose-50 border-rose-100 text-rose-900"
                : "bg-slate-900 border-slate-800 text-white"
            }`}
          >
            <div className="flex items-center gap-2.5">
              {t.type === "success" && <div className="w-2 h-2 bg-emerald-500 rounded-full animate-ping" />}
              {t.type === "error" && <div className="w-2 h-2 bg-rose-500 rounded-full" />}
              {t.type === "info" && <div className="w-2 h-2 bg-blue-400 rounded-full" />}
              <span className="text-xs font-medium">{t.message}</span>
            </div>
            <button onClick={() => setToasts((prev) => prev.filter((to) => to.id !== t.id))} className="text-slate-400 hover:text-slate-600 p-0.5">
              <X className="w-3.5 h-3.5" />
            </button>
          </div>
        ))}
      </div>

      {/* LEFT SIDEBAR NAVIGATION */}
      <aside id="crm-sidebar" className="w-64 bg-slate-900 flex flex-col border-r border-slate-800 shrink-0 text-slate-300">
        <div className="p-6 mb-2 flex items-center gap-3 border-b border-slate-800/60">
          <div className="w-8 h-8 bg-blue-600 rounded flex items-center justify-center text-white font-bold select-none">
            E
          </div>
          <div className="flex flex-col">
            <span className="text-white font-semibold text-base tracking-tight leading-none">Enterprise CRM</span>
            <span className="text-[10px] text-slate-500 font-mono mt-1">Unified Links Portal</span>
          </div>
        </div>

        <nav className="flex-1 px-4 py-3 space-y-1 overflow-y-auto">
          <button
            onClick={() => { setActiveTab("dashboard"); setSelectedItem(null); }}
            className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
              activeTab === "dashboard" ? "bg-slate-800 text-white" : "text-slate-400 hover:text-white hover:bg-slate-800/50"
            }`}
          >
            <LayoutDashboard className="w-4 h-4 text-blue-500" /> Dashboard
          </button>

          <button
            onClick={() => { setActiveTab("engagements"); setSelectedItem(null); }}
            className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
              activeTab === "engagements" ? "bg-slate-800 text-white animate-pulse" : "text-slate-400 hover:text-white hover:bg-slate-800/50"
            }`}
          >
            <Handshake className="w-4 h-4 text-sky-400" /> Engagements
          </button>

          <button
            onClick={() => { setActiveTab("interactions"); setSelectedItem(null); }}
            className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
              activeTab === "interactions" ? "bg-slate-800 text-white" : "text-slate-400 hover:text-white hover:bg-slate-800/50"
            }`}
          >
            <Clock className="w-4 h-4 text-amber-500" /> Interactions
          </button>

          <button
            onClick={() => { setActiveTab("contacts"); setSelectedItem(null); }}
            className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
              activeTab === "contacts" ? "bg-slate-800 text-white" : "text-slate-400 hover:text-white hover:bg-slate-800/50"
            }`}
          >
            <Users className="w-4 h-4 text-emerald-500" /> Contacts
          </button>

          <button
            onClick={() => { setActiveTab("entities"); setSelectedItem(null); }}
            className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
              activeTab === "entities" ? "bg-slate-800 text-white" : "text-slate-400 hover:text-white hover:bg-slate-800/50"
            }`}
          >
            <MapPin className="w-4 h-4 text-purple-500" /> Corporate Entities
          </button>

          <div className="pt-4 pb-2 border-t border-slate-800">
            <span className="px-3 text-[10px] font-bold text-slate-500 uppercase tracking-widest block mb-2">Workspace Vaults</span>
          </div>

          <button
            onClick={() => { setActiveTab("notes"); setSelectedItem(null); }}
            className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
              activeTab === "notes" ? "bg-slate-800 text-white" : "text-slate-400 hover:text-white hover:bg-slate-800/50"
            }`}
          >
            <Notebook className="w-4 h-4 text-indigo-400" /> Notes Ledger
          </button>

          <button
            onClick={() => { setActiveTab("documents"); setSelectedItem(null); }}
            className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
              activeTab === "documents" ? "bg-slate-800 text-white" : "text-slate-400 hover:text-white hover:bg-slate-800/50"
            }`}
          >
            <Paperclip className="w-4 h-4 text-violet-400" /> Document Vault
          </button>

           <button
            onClick={() => { setActiveTab("users"); setSelectedItem(null); }}
            className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
              activeTab === "users" ? "bg-slate-800 text-white" : "text-slate-400 hover:text-white hover:bg-slate-800/50"
            }`}
          >
            <UserCheck className="w-4 h-4 text-pink-400" /> User Management
          </button>

          {session?.role === "Senior Analyst" && (
            <button
              onClick={() => { setActiveTab("audit"); setSelectedItem(null); }}
              className={`w-full flex items-center gap-3 px-3 py-2.5 rounded-lg text-xs font-semibold tracking-wide transition-all ${
                activeTab === "audit" ? "bg-slate-800 text-white" : "text-amber-400/90 hover:text-white hover:bg-slate-800/50"
              }`}
            >
              <ShieldCheck className="w-4 h-4 text-amber-400" /> System Audit Ledger
            </button>
          )}
        </nav>

        {/* Operational Diagnostics statistics sidebar */}
        <div className="mx-4 p-3.5 bg-slate-950/80 border border-slate-800/50 rounded-xl mb-4 space-y-2 text-xs">
          <div className="flex justify-between items-center text-[10px] font-bold text-slate-500 uppercase tracking-wide">
            <span>Link Sync Diagnostic</span>
            <span className="w-2 h-2 rounded-full bg-emerald-500 animate-pulse" />
          </div>
          <div className="flex justify-between text-[11px] text-slate-400">
            <span>Total Logged Links</span>
            <strong className="text-white font-mono">{tags.length + notes.length + documents.length}</strong>
          </div>
        </div>

        {/* Security Session details */}
        <div className="p-4 border-t border-slate-800 bg-slate-950/30 flex items-center justify-between">
          <div className="flex items-center gap-3 overflow-hidden">
            <div className="w-9 h-9 rounded-full bg-blue-600 flex items-center justify-center font-bold text-xs text-white uppercase select-none shrink-0 border border-blue-500/30">
              {session.name ? session.name.split(" ").map((n) => n[0]).join("").slice(0, 2) : "DJ"}
            </div>
            <div className="overflow-hidden leading-none space-y-1">
              <p className="text-xs text-white font-semibold truncate">{session.name || "Operator"}</p>
              <p className="text-[10px] text-slate-500 truncate">{session.role || "Operator Unit"}</p>
            </div>
          </div>
          <button onClick={handleLogout} title="Log Out Console" className="p-1.5 hover:bg-rose-950/40 rounded text-slate-400 hover:text-rose-400">
            <LogOut className="w-4 h-4" />
          </button>
        </div>
      </aside>

      {/* MAIN CONTENT WRAPPER */}
      <main className="flex-1 flex flex-col overflow-hidden">

        {/* HEADER */}
        <header className="h-16 bg-white border-b border-slate-200 flex items-center justify-between px-8 shrink-0">
          <div className="flex items-center gap-4 flex-1 max-w-xl">
            <div className="relative w-full">
              <input
                type="text"
                placeholder="Search Interactions, Contacts, Corporate Accounts..."
                value={searchQuery}
                onChange={(e) => setSearchQuery(e.target.value)}
                className="w-full bg-slate-100 border border-transparent rounded-lg py-2 pl-9 pr-8 text-xs font-semibold text-slate-800 placeholder-slate-400 focus:bg-white focus:border-blue-500 outline-none transition"
              />
              <Search className="w-4 h-4 text-slate-400 absolute left-3 top-2.5" />
              {searchQuery && (
                <button onClick={() => setSearchQuery("")} className="absolute right-2.5 top-2.5 text-slate-400 hover:text-slate-600">
                  <X className="w-3.5 h-3.5" />
                </button>
              )}
            </div>
          </div>

          <div className="flex items-center gap-4 text-xs">
            <div className="relative">
              <button
                onClick={() => setIsNotificationOpen(!isNotificationOpen)}
                className="p-2 text-slate-400 hover:text-slate-600 relative hover:bg-slate-50 rounded-lg transition-colors duration-200"
                id="notification-bell-btn"
              >
                <Bell className="w-5 h-5" />
                {allNotifications.filter((n) => !n.isRead).length > 0 && (
                  <span className="absolute -top-1 -right-1 h-5 w-5 bg-rose-500 text-white rounded-full flex items-center justify-center text-[9px] font-bold border-2 border-white antialiased scale-95 animate-bounce">
                    {allNotifications.filter((n) => !n.isRead).length}
                  </span>
                )}
              </button>

              {isNotificationOpen && (
                <>
                  <div className="fixed inset-0 z-40" onClick={() => setIsNotificationOpen(false)} />
                  <div className="absolute right-0 mt-2 w-96 bg-white border border-slate-200 rounded-xl shadow-xl z-50 overflow-hidden py-1 text-xs">
                    <div className="px-4 py-3 bg-slate-50 border-b border-slate-100 flex justify-between items-center font-bold">
                      <span className="text-slate-700">Real-Time Event Feed ({allNotifications.filter((n) => !n.isRead).length} critical)</span>
                      {allNotifications.filter((n) => !n.isRead).length > 0 && (
                        <button
                          onClick={() => {
                            markAllNotificationsAsRead();
                            markAllVirtualNotifsAsRead(virtualNotifications.map((v) => v.id));
                          }}
                          className="text-blue-600 hover:text-blue-800 hover:underline text-[10px] transition-colors font-bold cursor-pointer"
                        >
                          Mark all as read
                        </button>
                      )}
                    </div>
                    <div className="max-h-80 overflow-y-auto divide-y divide-slate-100 leading-tight">
                      {allNotifications.length === 0 ? (
                        <div className="p-6 text-center text-slate-400 flex flex-col items-center justify-center gap-2">
                          <Bell className="w-8 h-8 text-slate-200" />
                          <p className="italic">No events logged in the workspace</p>
                        </div>
                      ) : (
                        allNotifications.map((notif) => (
                          <div
                            key={notif.id}
                            className={`px-4 py-3 hover:bg-slate-50/50 flex items-start justify-between gap-3 transition duration-150 ${
                              !notif.isRead ? "bg-blue-50/20" : ""
                            }`}
                          >
                            <div className="flex gap-2">
                              <span className="text-base shrink-0 select-none mt-0.5">
                                {notif.id.startsWith("notif-impending-") ? "⏰" : notif.actionType === "create" ? "🆕" : notif.actionType === "update" ? "🔄" : "🗑️"}
                              </span>
                              <div className="space-y-1">
                                <p className={`text-slate-700 text-[11px] leading-snug ${!notif.isRead ? "font-semibold" : ""}`}>
                                  {notif.message}
                                </p>
                                <div className="flex items-center gap-2 text-[10px] text-slate-400">
                                  <span>{new Date(notif.createdAt).toLocaleTimeString([], { hour: '2-digit', minute: '2-digit' })}</span>
                                  <span>•</span>
                                  <span className={`px-1.5 py-0.2 rounded text-[9px] uppercase font-bold tracking-wide ${
                                    notif.id.startsWith("notif-impending-") ? "bg-amber-100 text-amber-850" : "bg-slate-100 text-slate-500"
                                  }`}>
                                    {notif.entityTier}
                                  </span>
                                </div>
                              </div>
                            </div>
                            {!notif.isRead && (
                              <button
                                onClick={(e) => {
                                  e.stopPropagation();
                                  if (notif.id.startsWith("notif-impending-")) {
                                    markVirtualNotifAsRead(notif.id);
                                  } else {
                                    markNotificationAsRead(notif.id);
                                  }
                                }}
                                title="Mark as read"
                                className="p-1 hover:bg-emerald-50 text-emerald-600 rounded shrink-0 self-center transition-all cursor-pointer"
                              >
                                <Check className="w-3.5 h-3.5" />
                              </button>
                            )}
                          </div>
                        ))
                      )}
                    </div>
                  </div>
                </>
              )}
            </div>

            <div className="h-8 w-px bg-slate-200" />

            <button
              onClick={() => {
                if (activeTab === "contacts") setNewType("contact");
                else if (activeTab === "entities") setNewType("entity");
                else setNewType("interaction");
                setIsNewModalOpen(true);
              }}
              className="bg-blue-600 hover:bg-blue-700 text-white px-4 py-2 rounded-lg font-bold tracking-wide flex items-center gap-1.5 transition-all cursor-pointer"
            >
              <Plus className="w-4 h-4" /> Create Registry
            </button>
          </div>
        </header>

        {/* WORKSPACE TARGET PAGES */}
        <div className="flex-1 overflow-y-auto p-8 space-y-6">

          {/* TAB 1: EXECUTIVE DASHBOARD CONTAINER (NO CHARTS, DYNAMIC INTERACTIONS FEED) */}
          {activeTab === "dashboard" && (
            <div className="space-y-6 animate-in fade-in duration-300">

              {/* Impending / Due Soon Alerts Notification Banner */}
              {impendingInteractions.length > 0 && (
                <div className="bg-amber-50/70 border border-amber-200 p-4 rounded-xl flex items-start gap-3 shadow-sm text-xs leading-relaxed animate-in slide-in-from-top-4 duration-300">
                  <span className="p-2 bg-amber-100 text-amber-600 rounded-lg shrink-0">
                    <span className="relative flex h-3.5 w-3.5">
                      <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-amber-400 opacity-75"></span>
                      <span className="relative inline-flex rounded-full h-3.5 w-3.5 bg-amber-500 flex items-center justify-center text-[10px] text-white font-bold font-mono">⏰</span>
                    </span>
                  </span>
                  <div className="space-y-1.5 flex-1">
                    <h4 className="font-bold text-amber-850 font-mono text-[11px] uppercase tracking-wider">SYS ALIGNMENT FEED: {impendingInteractions.length} CRITICAL TOUCHPOINTS {"(< 24 HOURS)"}</h4>
                    <p className="text-slate-600 text-[11px]">
                      The system watchdog has detected scheduled or blocked Touchpoint interactions. Click any ticket block below to update state:
                    </p>
                    <div className="flex flex-wrap gap-2 pt-1 font-mono">
                      {impendingInteractions.map((item) => (
                        <div
                          key={item.id}
                          onClick={() => setSelectedItem({ dataType: "interaction", data: item })}
                          className="bg-white hover:bg-slate-50 border border-amber-200/80 px-2 py-1 rounded-md text-[10px] text-slate-700 font-semibold cursor-pointer shadow-xs transition hover:scale-102 flex items-center gap-1.5"
                        >
                          <span className={`w-1.5 h-1.5 rounded-full ${item.status === 'BLOCKED' ? 'bg-rose-500' : 'bg-blue-500'}`} />
                          <span className="font-bold text-[9px] uppercase text-slate-400">{item.status}</span>
                          <span className="text-slate-800 font-sans">{item.subject}</span>
                          <span className="text-[9px] text-amber-700 font-bold">({item.date})</span>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              )}

              {/* Dynamic KPI Rows */}
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-6 gap-6">
                <div onClick={() => setActiveTab("interactions")} className="bg-white p-5 rounded-xl border border-slate-200 shadow-sm cursor-pointer hover:border-slate-300 hover:-translate-y-0.5 transition duration-300">
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">Touchpoint Logs</p>
                      <p className="text-2xl font-extrabold text-slate-900 font-mono leading-none">{interactions.length}</p>
                    </div>
                    <div className="p-2 bg-amber-50 text-amber-600 rounded-lg"><Clock className="w-4 h-4" /></div>
                  </div>
                  <p className="mt-2 text-[9px] text-slate-400">Interactions stream</p>
                </div>

                <div onClick={() => setActiveTab("engagements")} className="bg-white p-5 rounded-xl border border-slate-200 shadow-sm cursor-pointer hover:border-slate-300 hover:-translate-y-0.5 transition duration-300">
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">Corporate Engagements</p>
                      <p className="text-2xl font-extrabold text-slate-900 font-mono leading-none">{engagements.length}</p>
                    </div>
                    <div className="p-2 bg-sky-50 text-sky-600 rounded-lg"><Handshake className="w-4 h-4" /></div>
                  </div>
                  <p className="mt-2 text-[9px] text-sky-600 font-bold font-mono">Active SOW state</p>
                </div>

                <div onClick={() => setActiveTab("contacts")} className="bg-white p-5 rounded-xl border border-slate-200 shadow-sm cursor-pointer hover:border-slate-300 hover:-translate-y-0.5 transition duration-300">
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">Roster Contacts</p>
                      <p className="text-2xl font-extrabold text-slate-900 font-mono leading-none">{contacts.length}</p>
                    </div>
                    <div className="p-2 bg-emerald-50 text-emerald-600 rounded-lg"><Users className="w-4 h-4" /></div>
                  </div>
                  <p className="mt-2 text-[9px] text-slate-400">Indexed stakeholder profiles</p>
                </div>

                <div onClick={() => setActiveTab("entities")} className="bg-white p-5 rounded-xl border border-slate-200 shadow-sm cursor-pointer hover:border-slate-300 hover:-translate-y-0.5 transition duration-300">
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">Global Entities</p>
                      <p className="text-2xl font-extrabold text-slate-900 font-mono leading-none">{entities.length}</p>
                    </div>
                    <div className="p-2 bg-purple-50 text-purple-600 rounded-lg"><MapPin className="w-4 h-4" /></div>
                  </div>
                  <p className="mt-2 text-[9px] text-slate-400">Strategic corporate entries</p>
                </div>

                <div onClick={() => setActiveTab("documents")} className="bg-white p-5 rounded-xl border border-slate-200 shadow-sm cursor-pointer hover:border-slate-300 hover:-translate-y-0.5 transition duration-300">
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">Document Vault</p>
                      <p className="text-2xl font-extrabold text-slate-900 font-mono leading-none">{documents.length}</p>
                    </div>
                    <div className="p-2 bg-violet-50 text-violet-600 rounded-lg"><Paperclip className="w-4 h-4" /></div>
                  </div>
                  <p className="mt-2 text-[9px] text-slate-400">Indexed links and items</p>
                </div>

                <div onClick={() => setActiveTab("notes")} className="bg-white p-5 rounded-xl border border-slate-200 shadow-sm cursor-pointer hover:border-slate-300 hover:-translate-y-0.5 transition duration-300">
                  <div className="flex justify-between items-start">
                    <div>
                      <p className="text-[10px] font-bold text-slate-400 uppercase tracking-wider mb-1">Notes Ledger</p>
                      <p className="text-2xl font-extrabold text-slate-900 font-mono leading-none">{notes.length}</p>
                    </div>
                    <div className="p-2 bg-indigo-50 text-indigo-600 rounded-lg"><Notebook className="w-4 h-4" /></div>
                  </div>
                  <p className="mt-2 text-[9px] text-indigo-600 font-bold">Active link ledger</p>
                </div>
              </div>

              {/* Quick Action Control Hub */}
              <div className="bg-slate-900 text-slate-100 p-6 rounded-xl border border-slate-800 shadow-lg flex flex-col md:flex-row justify-between items-center gap-6">
                <div>
                  <h3 className="text-white font-bold text-sm tracking-tight flex items-center gap-2">
                    <UserCheck className="w-4 h-4 text-pink-400" /> Enterprise Operator & Vault Control Hub
                  </h3>
                  <p className="text-xs text-slate-400 mt-1">
                    Manage system security operators, explore centralized notes, store documents and manage engagements directly.
                  </p>
                </div>
                <div className="flex flex-wrap gap-3 w-full md:w-auto shrink-0 justify-end">
                  <button onClick={() => { setActiveTab("engagements"); setSelectedItem(null); }} className="px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-200 hover:text-white rounded-lg text-xs font-bold transition flex items-center gap-2 border border-slate-700/60 cursor-pointer">
                    <Handshake className="w-3.5 h-3.5 text-sky-400" /> SOW Engagements
                  </button>
                  <button onClick={() => { setActiveTab("notes"); setSelectedItem(null); }} className="px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-200 hover:text-white rounded-lg text-xs font-bold transition flex items-center gap-2 border border-slate-700/60 cursor-pointer">
                    <Notebook className="w-3.5 h-3.5 text-indigo-400" /> Notes Ledger
                  </button>
                  <button onClick={() => { setActiveTab("documents"); setSelectedItem(null); }} className="px-4 py-2 bg-slate-800 hover:bg-slate-700 text-slate-200 hover:text-white rounded-lg text-xs font-bold transition flex items-center gap-2 border border-slate-700/60 cursor-pointer">
                    <Paperclip className="w-3.5 h-3.5 text-violet-400" /> Document Vault
                  </button>
                  <button onClick={() => { setActiveTab("users"); setSelectedItem(null); }} className="px-4 py-2 bg-pink-600 hover:bg-pink-700 text-white rounded-lg text-xs font-bold transition flex items-center gap-2 shadow-md cursor-pointer">
                    <UserCheck className="w-3.5 h-3.5" /> Operator Seats
                  </button>
                </div>
              </div>

              {/* TWO COLUMN ECOSYSTEM OVERVIEW (NO CHARTS) */}
              <div className="grid grid-cols-1 lg:grid-cols-3 gap-6">

                {/* Timeline activity list */}
                <div className="lg:col-span-2 bg-white rounded-xl border border-slate-200 shadow-sm flex flex-col min-h-[450px]">
                  <div className="px-6 py-4 border-b border-slate-100 flex justify-between items-center bg-slate-50/40">
                    <div>
                      <h3 className="font-bold text-slate-900 text-sm tracking-tight">Active Interactions Timeline</h3>
                      <p className="text-[10px] text-slate-400 mt-0.5">Continuous corporate activity logs and linked attributes stream</p>
                    </div>
                  </div>

                  <div className="p-6 space-y-6 overflow-y-auto max-h-[450px] divide-y divide-slate-100">
                    {interactions.map((item) => {
                      const intNotes = notes.filter((n) => n.linkedToType === "interaction" && n.linkedToId === item.id);
                      const intDocs = documents.filter((d) => d.linkedToType === "interaction" && d.linkedToId === item.id);
                      return (
                        <div key={item.id} className="pt-4 first:pt-0 group">
                          <div className="flex flex-col md:flex-row justify-between md:items-center gap-2 mb-1.5">
                            <div className="flex flex-wrap items-center gap-2">
                              {(() => {
                                const statusInfo = getStatusClasses(item.status);
                                return (
                                  <span className={`inline-flex items-center gap-1 px-1.5 py-0.5 rounded text-[8px] font-extrabold uppercase border shadow-sm ${statusInfo.bg}`}>
                                    <span className={`w-1 h-1 rounded-full ${statusInfo.dot}`} />
                                    {statusInfo.text}
                                  </span>
                                );
                              })()}
                              <span
                                onClick={() => setSelectedItem({ dataType: "interaction", data: item })}
                                className="font-bold text-xs text-slate-950 hover:text-blue-600 cursor-pointer block transition"
                              >
                                {item.subject}
                              </span>
                              <span className="px-1.5 py-0.5 rounded text-[8px] font-bold uppercase border bg-slate-50 border-slate-200 text-slate-500">
                                {item.type}
                              </span>
                            </div>
                            <span className="text-[10px] text-slate-400 font-mono">{item.date}</span>
                          </div>

                          <p className="text-xs text-slate-500 leading-relaxed mb-3">{item.summary}</p>

                          <div className="flex flex-wrap items-center justify-between gap-3">
                            <div className="flex flex-wrap gap-1">
                              {item.tagIds?.map((tId) => {
                                const activeTag = tags.find((t) => t.id === tId);
                                if (!activeTag) return null;
                                return (
                                  <span key={tId} className={`px-2 py-0.5 rounded text-[8px] font-bold uppercase tracking-wider border ${getColorClasses(activeTag.color)}`}>
                                    #{activeTag.name}
                                  </span>
                                );
                              })}
                              {item.tagIds?.length === 0 && <span className="text-[9px] text-slate-400 italic">No connected tags</span>}
                            </div>

                            <div className="flex gap-4 text-[10px] text-slate-400 font-semibold font-mono">
                              <span className="flex items-center gap-1"><FileText className="w-3.5 h-3.5" /> {intNotes.length} notes</span>
                              <span className="flex items-center gap-1"><Paperclip className="w-3.5 h-3.5" /> {intDocs.length} docs</span>
                              <span className="text-slate-500 font-sans font-bold">Assignee: {item.assignee}</span>
                            </div>
                          </div>
                        </div>
                      );
                    })}
                  </div>
                </div>

                {/* Right widgets: Tags index & Quick document links */}
                <div className="lg:col-span-1 space-y-6">

                  {/* Active Tags Widget */}
                  <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-5">
                    <h3 className="font-bold text-slate-900 text-sm tracking-tight mb-1">Ecosystem Tag Directory</h3>
                    <p className="text-[10px] text-slate-400 mb-4">Click index tag to filter search queries instantly</p>

                    <div className="flex flex-wrap gap-2">
                      {tags.map((t) => {
                        const occurrences = interactions.filter((i) => i.tagIds?.includes(t.id)).length;
                        return (
                          <button
                            key={t.id}
                            onClick={() => { setSearchQuery(t.name); showToast(`Filtering by tag #${t.name}`, "info"); }}
                            className={`inline-flex items-center gap-1.5 px-3 py-1 rounded-lg text-xs font-bold border hover:scale-[1.02] transition ${getColorClasses(t.color)}`}
                          >
                            #{t.name}
                            <span className="px-1.5 py-0.5 rounded-full text-[9px] bg-white/60 leading-none">{occurrences}</span>
                          </button>
                        );
                      })}
                    </div>
                  </div>

                  {/* Attachment Vault Tracker */}
                  <div className="bg-white rounded-xl border border-slate-200 shadow-sm p-5">
                    <h3 className="font-bold text-slate-900 text-sm tracking-tight mb-1">Central Document Vault</h3>
                    <p className="text-[10px] text-slate-400 mb-4">Centralized secure link records index</p>

                    <div className="space-y-2 max-h-60 overflow-y-auto modal-scroll pr-1">
                      {documents.map((doc) => (
                        <div key={doc.id} className="p-3 bg-slate-50 border border-slate-100 hover:bg-slate-100 transition rounded-lg text-xs flex justify-between items-center">
                          <div className="flex items-center gap-2 overflow-hidden mr-2">
                            <Paperclip className="w-4 h-4 text-slate-400 shrink-0" />
                            <div className="truncate">
                              <p className="font-bold text-slate-800 truncate" title={doc.title}>{doc.title}</p>
                              <span className="text-[9px] text-slate-400 font-mono">{doc.fileType} • {doc.fileSize}</span>
                            </div>
                          </div>
                          <button
                            onClick={() => showToast(`Simulated download trigger: ${doc.title}`, "success")}
                            className="text-blue-600 hover:underline text-[10px] font-bold shrink-0"
                          >
                            Download
                          </button>
                        </div>
                      ))}
                    </div>
                  </div>

                </div>

              </div>

            </div>
          )}

          {/* TAB 2: RICH KANBAN INTERACTIONS WORKSPACE */}
          {activeTab === "interactions" && (
            <div className="space-y-6 animate-in fade-in duration-300">

              {/* Impending / Due Soon Alerts Notification Banner */}
              {impendingInteractions.length > 0 && (
                <div className="bg-amber-50/70 border border-amber-200 p-4 rounded-xl flex items-start gap-3 shadow-sm text-xs leading-relaxed animate-in slide-in-from-top-4 duration-300">
                  <span className="p-2 bg-amber-100 text-amber-600 rounded-lg shrink-0">
                    <span className="relative flex h-3.5 w-3.5">
                      <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-amber-400 opacity-75"></span>
                      <span className="relative inline-flex rounded-full h-3.5 w-3.5 bg-amber-500 flex items-center justify-center text-[10px] text-white font-bold font-mono">⏰</span>
                    </span>
                  </span>
                  <div className="space-y-1.5 flex-1">
                    <h4 className="font-bold text-amber-850 font-mono text-[11px] uppercase tracking-wider">CRITICAL WORKSPACE ALERT: TOUCHPOINT DEADLINES WITHIN 24 HOURS</h4>
                    <p className="text-slate-600 text-[11px]">
                      The system watchdog has detected scheduled or blocked Touchpoint interactions currently due within 24 hours. Click any ticket block below to update state:
                    </p>
                    <div className="flex flex-wrap gap-2 pt-1 font-mono">
                      {impendingInteractions.map((item) => (
                        <div
                          key={item.id}
                          onClick={() => setSelectedItem({ dataType: "interaction", data: item })}
                          className="bg-white hover:bg-slate-50 border border-amber-200/80 px-2 py-1 rounded-md text-[10px] text-slate-700 font-semibold cursor-pointer shadow-xs transition hover:scale-102 flex items-center gap-1.5"
                        >
                          <span className={`w-1.5 h-1.5 rounded-full ${item.status === 'BLOCKED' ? 'bg-rose-500' : 'bg-blue-500'}`} />
                          <span className="font-bold text-[9px] uppercase text-slate-400">{item.status}</span>
                          <span className="text-slate-800 font-sans">{item.subject}</span>
                          <span className="text-[9px] text-amber-700 font-bold">({item.date})</span>
                        </div>
                      ))}
                    </div>
                  </div>
                </div>
              )}
              <div className="flex flex-col sm:flex-row sm:items-center justify-between gap-4 border-b border-slate-200 pb-3">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 tracking-tight">Interactions Alignment Board</h2>
                  <p className="text-xs text-slate-400">Process logs, meeting outcomes, and milestone schedules</p>
                </div>
                {/* View toggle switch */}
                <div className="flex items-center bg-slate-100 p-1 rounded-xl border border-slate-200/60 shadow-inner shrink-0 self-start sm:self-center">
                  <button
                    onClick={() => setInteractionsViewMode("kanban")}
                    className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-bold transition-all duration-200 ${
                      interactionsViewMode === "kanban"
                        ? "bg-white text-blue-600 shadow-sm border border-slate-200/50"
                        : "text-slate-500 hover:text-slate-900"
                    }`}
                  >
                    <LayoutGrid className="w-3.5 h-3.5" />
                    Board View
                  </button>
                  <button
                    onClick={() => setInteractionsViewMode("calendar")}
                    className={`flex items-center gap-1.5 px-3 py-1.5 rounded-lg text-xs font-bold transition-all duration-200 ${
                      interactionsViewMode === "calendar"
                        ? "bg-white text-blue-600 shadow-sm border border-slate-200/50"
                        : "text-slate-500 hover:text-slate-900"
                    }`}
                  >
                    <Calendar className="w-3.5 h-3.5" />
                    Calendar List
                  </button>
                </div>
              </div>

              {/* INTERACTION FILTER SYSTEM PANEL */}
              <div className="bg-white p-5 rounded-2xl border border-slate-200 shadow-sm space-y-4">
                <div className="flex items-center justify-between border-b border-slate-100 pb-3">
                  <div className="flex items-center gap-2">
                    <span className="p-1.5 bg-blue-50 text-blue-600 rounded-lg">
                      <Filter className="w-4 h-4" />
                    </span>
                    <div>
                      <h3 className="text-xs font-extrabold text-slate-800 uppercase tracking-wider font-mono">Workspace Query Filters</h3>
                      <p className="text-[10px] text-slate-400 font-medium">Refine alignment tasks by designated agents and temporal sequences</p>
                    </div>
                  </div>

                  {(interactionAssigneeFilter !== "ALL" || interactionTimeRangeFilter !== "ALL" || interactionStartDateFilter || interactionEndDateFilter) && (
                    <button
                      onClick={() => {
                        setInteractionAssigneeFilter("ALL");
                        setInteractionTimeRangeFilter("ALL");
                        setInteractionStartDateFilter("");
                        setInteractionEndDateFilter("");
                      }}
                      className="px-2.5 py-1 text-[10px] font-extrabold text-blue-600 hover:text-blue-700 bg-blue-50 hover:bg-blue-100 rounded-lg transition"
                    >
                      Reset All Filters
                    </button>
                  )}
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
                  {/* Filter 1: Assignee */}
                  <div className="space-y-1.5">
                    <label className="text-[11px] font-extrabold text-slate-600 uppercase tracking-wider font-mono block">Designated Assignee</label>
                    <div className="relative">
                      <select
                        value={interactionAssigneeFilter}
                        onChange={(e) => setInteractionAssigneeFilter(e.target.value)}
                        className="w-full bg-slate-50 border border-slate-200 hover:border-slate-300 rounded-xl px-3 py-2 text-xs font-bold text-slate-700 focus:outline-none focus:ring-2 focus:ring-blue-100 cursor-pointer appearance-none"
                      >
                        <option value="ALL">All Assignees (Reset)</option>
                        {ASSIGNEES.map((assignee) => (
                          <option key={assignee} value={assignee}>
                            👤 {assignee}
                          </option>
                        ))}
                      </select>
                      <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2.5 text-slate-400">
                        <ChevronDown className="w-4 h-4" />
                      </div>
                    </div>
                  </div>

                  {/* Filter 2: Time Range Selector */}
                  <div className="space-y-1.5">
                    <label className="text-[11px] font-extrabold text-slate-600 uppercase tracking-wider font-mono block">Sequence Time Range</label>
                    <div className="relative">
                      <select
                        value={interactionTimeRangeFilter}
                        onChange={(e) => setInteractionTimeRangeFilter(e.target.value)}
                        className="w-full bg-slate-50 border border-slate-200 hover:border-slate-300 rounded-xl px-3 py-2 text-xs font-bold text-slate-700 focus:outline-none focus:ring-2 focus:ring-blue-100 cursor-pointer appearance-none"
                      >
                        <option value="ALL">All Logged Actions</option>
                        <option value="TODAY">📅 Today's Agenda</option>
                        <option value="WEEK">⏱️ Within 7 Days (±)</option>
                        <option value="MONTH">🗓️ Within 30 Days (±)</option>
                        <option value="FUTURE">🚀 Scheduled Future Tasks</option>
                        <option value="CUSTOM">🛠️ Custom Temporal Span...</option>
                      </select>
                      <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-2.5 text-slate-400">
                        <ChevronDown className="w-4 h-4" />
                      </div>
                    </div>
                  </div>

                  {/* Filter 3: Custom Date Selection Spans */}
                  {interactionTimeRangeFilter === "CUSTOM" ? (
                    <div className="space-y-1.5">
                      <label className="text-[11px] font-extrabold text-slate-600 uppercase tracking-wider font-mono block">Custom Range Spans</label>
                      <div className="flex items-center gap-2">
                        <input
                          type="date"
                          value={interactionStartDateFilter}
                          onChange={(e) => setInteractionStartDateFilter(e.target.value)}
                          className="w-1/2 bg-slate-50 border border-slate-200 rounded-xl px-2.5 py-1.5 text-xs text-slate-700 font-bold focus:outline-none focus:ring-2 focus:ring-blue-100"
                        />
                        <span className="text-slate-400 font-bold text-xs">to</span>
                        <input
                          type="date"
                          value={interactionEndDateFilter}
                          onChange={(e) => setInteractionEndDateFilter(e.target.value)}
                          className="w-1/2 bg-slate-50 border border-slate-200 rounded-xl px-2.5 py-1.5 text-xs text-slate-700 font-bold focus:outline-none focus:ring-2 focus:ring-blue-100"
                        />
                      </div>
                    </div>
                  ) : (
                    <div className="flex items-center justify-center p-3 border border-dashed border-slate-150 rounded-xl bg-slate-50/50">
                      <p className="text-[10px] text-slate-400 italic text-center leading-normal">
                        Active Selection Matches: <strong className="text-blue-600">{filteredInteractions.length}</strong> items
                      </p>
                    </div>
                  )}
                </div>
              </div>

              {/* Batch Action Helper Header Control */}
              <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 bg-slate-50 p-4 rounded-xl border border-slate-200">
                <div className="flex items-center gap-3">
                  <div className="flex items-center h-5">
                    <input
                      id="select-all-interactions"
                      type="checkbox"
                      checked={filteredInteractions.length > 0 && selectedInteractionIds.length === filteredInteractions.length}
                      onChange={(e) => {
                        if (e.target.checked) {
                          setSelectedInteractionIds(filteredInteractions.map((i) => i.id));
                        } else {
                          setSelectedInteractionIds([]);
                        }
                      }}
                      className="w-4 h-4 cursor-pointer rounded border-slate-300 text-blue-605 focus:ring-blue-500"
                    />
                  </div>
                  <div>
                    <label htmlFor="select-all-interactions" className="text-xs font-bold text-slate-800 cursor-pointer select-none">
                      Select All Workspace Interactions
                    </label>
                    <p className="text-[10px] text-slate-500 font-medium">
                      {selectedInteractionIds.length} of {filteredInteractions.length} alignment cards currently selected
                    </p>
                  </div>
                </div>

                {selectedInteractionIds.length > 0 && (
                  <button
                    onClick={() => setSelectedInteractionIds([])}
                    className="px-3 py-1 text-[11px] font-extrabold text-slate-500 hover:text-rose-600 hover:bg-rose-50 rounded-lg border border-slate-200 hover:border-rose-100 transition"
                  >
                    Clear Slate Selection ({selectedInteractionIds.length})
                  </button>
                )}
              </div>

              {interactionsViewMode === "kanban" ? (
                <div className="grid grid-cols-1 md:grid-cols-4 gap-6 animate-in fade-in duration-300">

                  {/* Column Generator */}
                  {(["IN PROGRESS", "SCHEDULED", "COMPLETED", "BLOCKED"] as Interaction["status"][]).map((status) => {
                    const items = filteredInteractions.filter((i) => i.status === status);
                    const statusColors = {
                      "IN PROGRESS": { border: "border-amber-200", bg: "bg-amber-500", text: "text-amber-700" },
                      "SCHEDULED": { border: "border-blue-200", bg: "bg-blue-600", text: "text-blue-700" },
                      "COMPLETED": { border: "border-emerald-200", bg: "bg-emerald-600", text: "text-emerald-700" },
                      "BLOCKED": { border: "border-rose-200", bg: "bg-rose-500", text: "text-rose-700" }
                    }[status];

                    return (
                      <div key={status} className="bg-slate-100/60 border border-slate-200 p-4 rounded-xl flex flex-col space-y-4">
                        <div className="flex justify-between items-center pb-2 border-b border-slate-200">
                          <span className={`text-xs font-bold uppercase tracking-wider flex items-center gap-1.5 ${statusColors.text}`}>
                            <span className={`w-2 h-2 rounded-full ${statusColors.bg} animate-pulse`} />
                            {status} ({items.length})
                          </span>
                        </div>

                        <div className="space-y-3 flex-1 overflow-y-auto max-h-[500px]">
                          {items.length === 0 ? (
                            <p className="text-[11px] text-slate-400 italic text-center py-6 border border-dashed border-slate-200 rounded-lg">No logged items</p>
                          ) : (
                            items.map((item) => {
                              const intNotes = notes.filter((n) => n.linkedToType === "interaction" && n.linkedToId === item.id);
                              const intDocs = documents.filter((d) => d.linkedToType === "interaction" && d.linkedToId === item.id);
                              const cardStatus = getStatusClasses(item.status);
                              const isSelected = selectedInteractionIds.includes(item.id);
                              const isImpending = impendingInteractions.some((imp) => imp.id === item.id);
                              return (
                                <div
                                  key={item.id}
                                  onClick={() => setSelectedItem({ dataType: "interaction", data: item })}
                                  className={`p-3.5 border rounded-lg cursor-pointer shadow-sm transition-all duration-250 ${
                                    isSelected
                                      ? "bg-blue-50/50 border-blue-400 ring-2 ring-blue-100/50 scale-[0.99] shadow-inner"
                                      : isImpending
                                        ? "bg-amber-50/30 border-amber-300 ring-1 ring-amber-100/40 hover:border-amber-400"
                                        : `bg-white hover:border-slate-400 ${cardStatus.border}`
                                  }`}
                                >
                                  <div className="flex justify-between items-center text-[9px] text-slate-400 mb-1.5 font-mono">
                                    <div className="flex items-center gap-1.5 flex-wrap">
                                      <input
                                        type="checkbox"
                                        checked={isSelected}
                                        onClick={(e) => e.stopPropagation()}
                                        onChange={(e) => {
                                          setSelectedInteractionIds((prev) =>
                                            e.target.checked
                                              ? [...prev, item.id]
                                              : prev.filter((id) => id !== item.id)
                                          );
                                        }}
                                        className="w-3.5 h-3.5 rounded border-slate-300 text-blue-600 focus:ring-blue-500 cursor-pointer shrink-0"
                                      />
                                      <span className="font-bold text-slate-500 uppercase">{item.type}</span>
                                      <span className={`inline-flex items-center gap-1 px-1 py-0.2 rounded text-[7px] font-extrabold border uppercase shadow-sm ${cardStatus.bg}`}>
                                        <span className={`w-1 h-1 rounded-full ${cardStatus.dot}`} />
                                        {cardStatus.text}
                                      </span>
                                      {isImpending && (
                                        <span className="inline-flex items-center gap-1 px-1.5 py-0.2 bg-amber-100 text-amber-850 rounded text-[7.5px] font-extrabold border border-amber-300 uppercase tracking-wider animate-pulse">
                                          ⏰ Due soon
                                        </span>
                                      )}
                                    </div>
                                    <span className="font-mono text-slate-400">{item.date}</span>
                                  </div>
                                  <h4 className="font-bold text-slate-900 text-xs mb-1 hover:text-blue-600 transition leading-snug">{item.subject}</h4>
                                  <p className="text-[10px] text-slate-500 font-semibold">{item.client}</p>

                                  <div className="flex flex-wrap gap-1 mt-2 mb-3">
                                    {item.tagIds?.map((tId) => {
                                      const activeTag = tags.find((t) => t.id === tId);
                                      if (!activeTag) return null;
                                      return (
                                        <span key={tId} className={`px-1.5 py-0.5 rounded-[4px] text-[8px] font-bold uppercase tracking-wider border ${getColorClasses(activeTag.color)}`}>
                                          {activeTag.name}
                                        </span>
                                      );
                                    })}
                                  </div>

                                  <div className="pt-2.5 border-t border-slate-100 flex justify-between items-center text-[9px] text-slate-400 font-bold">
                                    <span>{item.assignee}</span>
                                    <span className="inline-flex items-center gap-1 bg-slate-50 px-1.5 py-0.5 rounded border border-slate-150">
                                      📝{intNotes.length} 📎{intDocs.length}
                                    </span>
                                  </div>
                                </div>
                              );
                            })
                          )}
                        </div>
                      </div>
                    );
                  })}
                </div>
              ) : (
                <div className="space-y-6 animate-in fade-in duration-300">
                  {/* REAL CALENDAR MONTH CONTROLLER & HEADER PANEL */}
                  {(() => {
                    const daysInMonth = new Date(calendarYear, calendarMonth + 1, 0).getDate();
                    const firstDayIndex = new Date(calendarYear, calendarMonth, 1).getDay();
                    const prevMonthDays = new Date(calendarYear, calendarMonth, 0).getDate();

                    const prevMonthOffsetDays = [];
                    for (let i = firstDayIndex - 1; i >= 0; i--) {
                      prevMonthOffsetDays.push({
                        dayNum: prevMonthDays - i,
                        monthType: "prev",
                        year: calendarMonth === 0 ? calendarYear - 1 : calendarYear,
                        month: calendarMonth === 0 ? 11 : calendarMonth - 1,
                      });
                    }

                    const activeMonthDays = [];
                    for (let d = 1; d <= daysInMonth; d++) {
                      activeMonthDays.push({
                        dayNum: d,
                        monthType: "current",
                        year: calendarYear,
                        month: calendarMonth,
                      });
                    }

                    const totalCellsSoFar = prevMonthOffsetDays.length + activeMonthDays.length;
                    const remainingCellsCount = (totalCellsSoFar % 7 === 0) ? 0 : 7 - (totalCellsSoFar % 7);
                    const nextMonthOffsetDays = [];
                    for (let d = 1; d <= remainingCellsCount; d++) {
                      nextMonthOffsetDays.push({
                        dayNum: d,
                        monthType: "next",
                        year: calendarMonth === 11 ? calendarYear + 1 : calendarYear,
                        month: calendarMonth === 11 ? 0 : calendarMonth + 1,
                      });
                    }

                    const allCalendarDays = [...prevMonthOffsetDays, ...activeMonthDays, ...nextMonthOffsetDays];
                    const MONTH_NAMES = [
                      "January", "February", "March", "April", "May", "June",
                      "July", "August", "September", "October", "November", "December"
                    ];

                    const handlePrevMonth = () => {
                      if (calendarMonth === 0) {
                        setCalendarMonth(11);
                        setCalendarYear(prev => prev - 1);
                      } else {
                        setCalendarMonth(prev => prev - 1);
                      }
                    };

                    const handleNextMonth = () => {
                      if (calendarMonth === 11) {
                        setCalendarMonth(0);
                        setCalendarYear(prev => prev + 1);
                      } else {
                        setCalendarMonth(prev => prev + 1);
                      }
                    };

                    // Compute month active stats
                    const monthStartDateStr = `${calendarYear}-${String(calendarMonth + 1).padStart(2, "0")}-01`;
                    const monthEndDateStr = `${calendarYear}-${String(calendarMonth + 1).padStart(2, "0")}-${String(daysInMonth).padStart(2, "0")}`;
                    const monthActiveInteractions = filteredInteractions.filter(item => {
                      return item.date >= monthStartDateStr && item.date <= monthEndDateStr;
                    });
                    const completedInMonth = monthActiveInteractions.filter(i => i.status === "COMPLETED").length;
                    const pendingInMonth = monthActiveInteractions.filter(i => i.status === "IN PROGRESS" || i.status === "SCHEDULED").length;

                    return (
                      <div className="space-y-4">
                        {/* CALENDAR CONTROLS BAR */}
                        <div className="flex flex-col md:flex-row items-stretch md:items-center justify-between gap-4 bg-white p-4 rounded-2xl border border-slate-200/80 shadow-sm">
                          <div className="flex items-center gap-3">
                            <div className="w-10 h-10 rounded-xl bg-blue-50 text-blue-600 flex items-center justify-center">
                              <Calendar className="w-5 h-5 animate-pulse" />
                            </div>
                            <div>
                              <h3 className="text-sm font-extrabold text-slate-800 tracking-tight flex items-center gap-2">
                                <span className="font-mono text-xs text-blue-650 font-black">CHRONO-FLOW</span>
                                <span className="text-slate-300">•</span>
                                <span>{MONTH_NAMES[calendarMonth]} {calendarYear}</span>
                              </h3>
                              <p className="text-[10px] text-slate-400 font-semibold leading-none mt-1">
                                Monthly sequence analysis • <strong className="text-blue-600">{monthActiveInteractions.length} aligned tasks</strong> in this range
                              </p>
                            </div>
                          </div>

                          {/* Navigation Buttons */}
                          <div className="flex flex-wrap items-center gap-1.5 self-center">
                            <button
                              onClick={() => setCalendarYear(prev => prev - 1)}
                              title="Previous Year"
                              className="p-1.5 hover:bg-slate-100 rounded-lg border border-slate-200 text-slate-500 hover:text-slate-950 transition"
                            >
                              <ChevronsLeft className="w-4 h-4" />
                            </button>
                            <button
                              onClick={handlePrevMonth}
                              title="Previous Month"
                              className="p-1.5 hover:bg-slate-100 rounded-lg border border-slate-200 text-slate-600 hover:text-slate-955 transition flex items-center gap-1"
                            >
                              <ChevronLeft className="w-4 h-4" />
                            </button>

                            <button
                              onClick={() => {
                                setCalendarYear(2026);
                                setCalendarMonth(5); // June is index 5
                              }}
                              className="px-3 py-1.5 text-xs font-extrabold font-sans bg-slate-900 border border-slate-800 hover:bg-slate-800 text-slate-100 rounded-lg transition"
                            >
                              Target Month (June '26)
                            </button>

                            <button
                              onClick={handleNextMonth}
                              title="Next Month"
                              className="p-1.5 hover:bg-slate-100 rounded-lg border border-slate-200 text-slate-600 hover:text-slate-955 transition flex items-center gap-1"
                            >
                              <ChevronRight className="w-4 h-4" />
                            </button>
                            <button
                              onClick={() => setCalendarYear(prev => prev + 1)}
                              title="Next Year"
                              className="p-1.5 hover:bg-slate-100 rounded-lg border border-slate-200 text-slate-500 hover:text-slate-950 transition"
                            >
                              <ChevronsRight className="w-4 h-4" />
                            </button>
                          </div>

                          {/* Mini statistics info */}
                          <div className="hidden lg:flex items-center gap-4 text-[10.5px] border-l border-slate-150 pl-4 shrink-0">
                            <div className="space-y-0.5">
                              <span className="text-slate-400 block font-semibold uppercase text-[8px] font-mono leading-none">Completed</span>
                              <strong className="text-emerald-600 font-extrabold">{completedInMonth} actions</strong>
                            </div>
                            <div className="space-y-0.5">
                              <span className="text-slate-400 block font-semibold uppercase text-[8px] font-mono leading-none">Active Pending</span>
                              <strong className="text-amber-600 font-extrabold">{pendingInMonth} tasks</strong>
                            </div>
                          </div>
                        </div>

                        {/* CALENDAR WEEKDAY HEADER */}
                        <div className="bg-white rounded-2xl border border-slate-200 shadow-sm overflow-visible p-4">
                          <div className="grid grid-cols-7 gap-1 text-center border-b border-slate-100 pb-2 mb-1.5">
                            {["Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"].map((dayName) => (
                              <div key={dayName} className="text-[10px] font-extrabold text-slate-500 tracking-wider uppercase font-mono py-1">
                                <span className="hidden sm:inline">{dayName}</span>
                                <span className="inline sm:hidden">{dayName.slice(0, 3)}</span>
                              </div>
                            ))}
                          </div>

                          {/* MONTH GRID CELLS SECTION */}
                          <div className="grid grid-cols-7 gap-1 bg-slate-100/50 rounded-xl overflow-visible p-1">
                            {allCalendarDays.map((day, cellIndex) => {
                              const isCurrentMonth = day.monthType === "current";
                              const isToday = day.dayNum === 7 && day.month === 5 && day.year === 2026; // June 7, 2026 from local metadata

                              // Format date as YYYY-MM-DD
                              const dateStr = `${day.year}-${String(day.month + 1).padStart(2, "0")}-${String(day.dayNum).padStart(2, "0")}`;
                              const dayInteractions = filteredInteractions.filter(i => i.date === dateStr);

                              return (
                                <div
                                  key={`${day.year}-${day.month}-${day.dayNum}-${cellIndex}`}
                                  className={`min-h-[115px] p-2 rounded-lg border transition-all flex flex-col justify-between overflow-visible relative ${
                                    isCurrentMonth
                                      ? isToday
                                        ? "bg-blue-50/50 border-blue-400 ring-2 ring-blue-100/40 shadow-inner"
                                        : "bg-white border-slate-200/70 hover:bg-slate-50/60"
                                      : "bg-slate-50/70 text-slate-350 border-slate-200/40 opacity-55"
                                  }`}
                                >
                                  {/* Cell Date Header and indicator */}
                                  <div className="flex items-center justify-between mb-1.5 select-none overflow-visible">
                                    <span className={`text-[10.5px] font-black font-mono leading-none w-5 h-5 flex items-center justify-center rounded-full ${
                                      isToday
                                        ? "bg-blue-600 text-white font-black"
                                        : isCurrentMonth
                                          ? "text-slate-800"
                                          : "text-slate-400 font-semibold"
                                    }`}>
                                      {day.dayNum}
                                    </span>

                                    {isToday && (
                                      <span className="text-[7.5px] font-black uppercase tracking-widest text-blue-600 bg-blue-100/70 px-1 rounded border border-blue-200/50">
                                        Today
                                      </span>
                                    )}

                                    {dayInteractions.length > 0 && !isToday && (
                                      <span className="w-1.5 h-1.5 rounded-full bg-slate-400 shrink-0" />
                                    )}
                                  </div>

                                  {/* List of day interactions inside cell */}
                                  <div className="flex-1 space-y-1 overflow-visible">
                                    {dayInteractions.map((item) => {
                                      const isSelected = selectedInteractionIds.includes(item.id);
                                      const cardStatus = getStatusClasses(item.status);
                                      const intNotes = notes.filter((n) => n.linkedToType === "interaction" && n.linkedToId === item.id);
                                      const intDocs = documents.filter((d) => d.linkedToType === "interaction" && d.linkedToId === item.id);

                                      // Optimize flyout alignment to prevent offscreen clipping at grid edges
                                      const colIdx = cellIndex % 7;
                                      let alignClass = "left-1/2 -translate-x-1/2";
                                      if (colIdx <= 1) {
                                        alignClass = "left-0";
                                      } else if (colIdx >= 5) {
                                        alignClass = "right-0";
                                      }

                                      return (
                                        <div
                                          key={item.id}
                                          className="relative group/intpill"
                                          onClick={(e) => {
                                            e.stopPropagation();
                                            setSelectedItem({ dataType: "interaction", data: item });
                                          }}
                                        >
                                          {/* Item Pill */}
                                          <div className={`px-1.5 py-0.5 rounded text-[9px] font-extrabold cursor-pointer border flex items-center justify-between gap-1 transition-all hover:scale-[1.02] shadow-[0_1px_2px_rgba(0,0,0,0.03)] select-none ${
                                            isSelected
                                              ? "bg-blue-100 border-blue-400 text-blue-800"
                                              : `${cardStatus.bg} ${cardStatus.border} hover:border-slate-450`
                                          }`}>
                                            <span className="truncate flex-1 leading-snug">{item.subject}</span>
                                            <input
                                              type="checkbox"
                                              checked={isSelected}
                                              onClick={(e) => e.stopPropagation()}
                                              onChange={(e) => {
                                                setSelectedInteractionIds((prev) =>
                                                  e.target.checked
                                                    ? [...prev, item.id]
                                                    : prev.filter((id) => id !== item.id)
                                                );
                                              }}
                                              className="w-2.5 h-2.5 rounded border-slate-300 text-blue-600 focus:ring-blue-500 cursor-pointer shrink-0"
                                            />
                                          </div>

                                          {/* OVERLAY INTERACTIVE POPUP FLYOUT FOR PRECISE DETAILS */}
                                          <div className={`absolute bottom-full mb-2 w-72 p-4 bg-slate-950 text-slate-100 border border-slate-800 shadow-[0_20px_50px_rgba(0,0,0,0.8)] rounded-2xl invisible opacity-0 scale-95 origin-bottom transition-all duration-200 group-hover/intpill:visible group-hover/intpill:opacity-100 group-hover/intpill:scale-100 z-[95] pointer-events-none ${alignClass}`}>
                                            <div className="space-y-3.5 text-left">
                                              <div className="flex items-center justify-between pb-2 border-b border-slate-900">
                                                <span className="text-[8px] font-black font-mono text-sky-400 uppercase tracking-widest">{item.type}</span>
                                                <span className={`inline-flex items-center gap-1 px-1.5 py-0.2 rounded text-[7.5px] font-extrabold border uppercase shadow-sm ${cardStatus.bg}`}>
                                                  <span className={`w-1 h-1 rounded-full ${cardStatus.dot}`} />
                                                  {cardStatus.text}
                                                </span>
                                              </div>

                                              <h4 className="text-white font-extrabold text-xs tracking-tight whitespace-normal leading-normal">{item.subject}</h4>

                                              <div className="space-y-1 text-[10px]">
                                                <p className="text-slate-400 font-semibold">Client • <span className="font-extrabold text-white">{item.client}</span></p>
                                                <p className="text-slate-400 font-semibold">Assignee • <span className="font-extrabold text-slate-300">{item.assignee}</span></p>
                                                <p className="text-slate-400 font-semibold">Scheduled Date • <span className="font-bold text-sky-405 font-mono">{item.date}</span></p>
                                              </div>

                                              {item.summary && (
                                                <div className="pt-2 border-t border-slate-850/80 text-[10.5px] space-y-1">
                                                  <span className="text-[8px] font-bold text-slate-500 uppercase tracking-wider block font-mono">Summary & Outcomes</span>
                                                  <p className="text-slate-200 leading-relaxed italic whitespace-normal font-medium">
                                                    "{item.summary}"
                                                  </p>
                                                </div>
                                              )}

                                              {/* Project Tag Labels */}
                                              {item.tagIds && item.tagIds.length > 0 && (
                                                <div className="pt-2.5 border-t border-slate-850/80 flex flex-wrap gap-1">
                                                  {item.tagIds.map((tId) => {
                                                    const activeTag = tags.find((t) => t.id === tId);
                                                    if (!activeTag) return null;
                                                    return (
                                                      <span key={tId} className={`px-1.5 py-0.5 rounded text-[7px] font-bold uppercase tracking-wider border ${getColorClasses(activeTag.color)}`}>
                                                        {activeTag.name}
                                                      </span>
                                                    );
                                                  })}
                                                </div>
                                              )}

                                              {/* Associated dynamic records stats */}
                                              {(intNotes.length > 0 || intDocs.length > 0) && (
                                                <div className="pt-2 border-t border-slate-850/80 grid grid-cols-2 gap-2 text-[9px]">
                                                  <div className="bg-slate-900 border border-slate-850 p-2 rounded">
                                                    <span className="text-slate-500 font-bold uppercase text-[7px] block font-mono">Linked Notes</span>
                                                    <span className="text-white font-extrabold font-mono">📝 {intNotes.length} logs</span>
                                                  </div>
                                                  <div className="bg-slate-900 border border-slate-850 p-2 rounded">
                                                    <span className="text-slate-500 font-bold uppercase text-[7px] block font-mono">Files Attached</span>
                                                    <span className="text-white font-extrabold font-mono">📎 {intDocs.length} files</span>
                                                  </div>
                                                </div>
                                              )}
                                            </div>
                                          </div>
                                        </div>
                                      );
                                    })}
                                  </div>

                                  {/* Quick add prompt helper inside cell */}
                                  <button
                                    onClick={() => {
                                      setIntForm((prev) => ({ ...prev, date: dateStr }));
                                      // Focus search or scroll down to Add block
                                      const createToggle = document.getElementById("log-interaction-heading") || document.getElementById("select-all-interactions");
                                      if (createToggle) {
                                        createToggle.scrollIntoView({ behavior: "smooth" });
                                      }
                                    }}
                                    className="text-[8px] font-mono hover:text-blue-600 text-slate-300 transition mt-1 block w-full text-right self-end"
                                    title="Add alignment interaction for this day"
                                  >
                                    + Log
                                  </button>
                                </div>
                              );
                            })}
                          </div>
                        </div>
                      </div>
                    );
                  })()}
                </div>
              )}
            </div>
          )}

          {/* TAB 3: CONTACTS DIRECTORY RASTER */}
          {activeTab === "contacts" && (
            <div className="space-y-6 animate-in fade-in duration-300">
              <div className="flex justify-between items-center border-b border-slate-200 pb-3">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 tracking-tight">Active Contacts Directory</h2>
                  <p className="text-xs text-slate-400">Enterprise stakeholder profiles, dynamic records, and communications links</p>
                </div>
              </div>

              {/* Batch Action Helper Header Control */}
              <div className="flex flex-col sm:flex-row items-start sm:items-center justify-between gap-3 bg-slate-50 p-4 rounded-xl border border-slate-200">
                <div className="flex items-center gap-3">
                  <div className="flex items-center h-5">
                    <input
                      id="select-all-contacts"
                      type="checkbox"
                      checked={contacts.length > 0 && selectedContactIds.length === contacts.length}
                      onChange={(e) => {
                        if (e.target.checked) {
                          setSelectedContactIds(contacts.map((c) => c.id));
                        } else {
                          setSelectedContactIds([]);
                        }
                      }}
                      className="w-4 h-4 cursor-pointer rounded border-slate-300 text-emerald-601 focus:ring-emerald-500"
                    />
                  </div>
                  <div>
                    <label htmlFor="select-all-contacts" className="text-xs font-bold text-slate-800 cursor-pointer select-none">
                      Select All Active Contacts
                    </label>
                    <p className="text-[10px] text-slate-500 font-medium">
                      {selectedContactIds.length} of {contacts.length} stakeholder profiles currently selected
                    </p>
                  </div>
                </div>

                {selectedContactIds.length > 0 && (
                  <button
                    onClick={() => setSelectedContactIds([])}
                    className="px-3 py-1 text-[11px] font-extrabold text-slate-500 hover:text-rose-600 hover:bg-rose-50 rounded-lg border border-slate-200 hover:border-rose-100 transition"
                  >
                    Clear Slate Selection ({selectedContactIds.length})
                  </button>
                )}
              </div>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {contacts.map((con) => {
                  const initialName = con.name.split(" ").map((n) => n[0]).join("").slice(0, 2);
                  const conNotes = notes.filter((n) => n.linkedToType === "contact" && n.linkedToId === con.id);
                  const conDocs = documents.filter((d) => d.linkedToType === "contact" && d.linkedToId === con.id);
                  const linkedEntity = entities.find((ent) =>
                    ent.name.toLowerCase() === con.company.toLowerCase() ||
                    ent.name.toLowerCase().includes(con.company.toLowerCase()) ||
                    con.company.toLowerCase().includes(ent.name.toLowerCase())
                  );
                  const isSelected = selectedContactIds.includes(con.id);

                  return (
                    <div
                      key={con.id}
                      onClick={() => setSelectedItem({ dataType: "contact", data: con })}
                      className={`border rounded-xl p-5 cursor-pointer shadow-sm transition-all duration-250 flex flex-col justify-between ${
                        isSelected
                          ? "bg-emerald-50/50 border-emerald-400 ring-2 ring-emerald-100/50 scale-[0.99] shadow-inner"
                          : "bg-white border-slate-200 hover:border-emerald-400"
                      }`}
                    >
                      <div>
                        <div className="flex items-start gap-3">
                          {/* Checked indicator toggle */}
                          <div className="flex items-center h-10 shrink-0">
                            <input
                              type="checkbox"
                              checked={isSelected}
                              onClick={(e) => e.stopPropagation()}
                              onChange={(e) => {
                                setSelectedContactIds((prev) =>
                                  e.target.checked
                                    ? [...prev, con.id]
                                    : prev.filter((id) => id !== con.id)
                                );
                              }}
                              className="w-4 h-4 rounded border-slate-300 text-emerald-600 focus:ring-emerald-500 cursor-pointer"
                            />
                          </div>

                          <div className="w-10 h-10 rounded-full bg-slate-100 flex items-center justify-center font-bold text-slate-600 text-sm border border-slate-200 uppercase select-none shrink-0">
                            {initialName || "C"}
                          </div>
                          <div className="min-w-0 flex-1 relative">
                            <div className="group/name relative inline-block max-w-full">
                              <h3 className="font-bold text-slate-900 text-sm hover:text-blue-600 transition truncate cursor-help flex items-center gap-1">
                                {con.name}
                                <span className="w-1.5 h-1.5 rounded-full bg-emerald-500 animate-pulse ml-1" />
                              </h3>

                              {/* STAKEHOLDER FLOATING HOVER CARD */}
                              <div className="absolute invisible opacity-0 scale-95 origin-top-left translate-y-1.5 group-hover/name:visible group-hover/name:opacity-100 group-hover/name:scale-100 group-hover/name:translate-y-0 transition-all duration-200 z-[70] left-0 top-full mt-2 w-80 p-5 bg-slate-950 text-slate-100 border border-slate-800 shadow-2xl rounded-2xl cursor-default pointer-events-none text-xs space-y-4">
                                <div className="space-y-1.5">
                                  <div className="flex items-center justify-between">
                                    <span className="text-[9px] font-bold text-emerald-400 uppercase tracking-widest font-mono">Stakeholder Record Brief</span>
                                    <span className="px-2 py-0.5 bg-emerald-900/40 text-emerald-300 border border-emerald-800/40 rounded text-[8px] font-bold uppercase tracking-wider">
                                      Active Sync
                                    </span>
                                  </div>
                                  <h4 className="text-white font-extrabold text-sm tracking-tight leading-snug">{con.name}</h4>
                                  <p className="text-slate-310 font-semibold">{con.role} • <span className="font-extrabold text-white">{con.company}</span></p>
                                </div>

                                {linkedEntity ? (
                                  <div className="pt-3 border-t border-slate-800 space-y-2">
                                    <span className="text-[9px] font-bold text-purple-400 uppercase tracking-widest font-mono block">Associated Organization Details</span>
                                    <div className="grid grid-cols-2 gap-2 text-[10px] text-slate-200">
                                      <div className="flex flex-col bg-slate-900 p-1.5 rounded border border-slate-850">
                                        <span className="text-slate-500 font-bold uppercase text-[7px] tracking-wider">HQ Location</span>
                                        <span className="font-semibold text-slate-100 truncate">{linkedEntity.location}</span>
                                      </div>
                                      <div className="flex flex-col bg-slate-900 p-1.5 rounded border border-slate-850">
                                        <span className="text-slate-500 font-bold uppercase text-[7px] tracking-wider">Account Tier</span>
                                        <span className="font-semibold text-slate-100 truncate">{linkedEntity.tier} Account</span>
                                      </div>
                                      <div className="flex flex-col col-span-2 bg-slate-900 p-1.5 rounded border border-slate-850">
                                        <span className="text-slate-500 font-bold uppercase text-[7px] tracking-wider">Sector Industry</span>
                                        <span className="font-semibold text-slate-100 truncate">{linkedEntity.industry}</span>
                                      </div>
                                    </div>
                                  </div>
                                ) : (
                                  <div className="pt-3 border-t border-slate-800 space-y-1">
                                    <span className="text-[9px] font-bold text-slate-500 uppercase tracking-widest font-mono block">Primary Organization</span>
                                    <div className="bg-slate-900/50 p-2 rounded border border-slate-850/50">
                                      <p className="text-[10px] text-slate-300 italic truncate">{con.company}</p>
                                      <span className="text-[8px] text-slate-500 block mt-0.5">Note: This organization has no separate corporate profile registered yet.</span>
                                    </div>
                                  </div>
                                )}

                                <div className="pt-3 border-t border-slate-800 flex justify-between items-center text-[9px] text-slate-400 font-mono">
                                  <span>📝 {conNotes.length} Linked Notes</span>
                                  <span>📎 {conDocs.length} Linked Docs</span>
                                </div>

                                {conNotes.length > 0 ? (
                                  <div className="pt-3 border-t border-slate-800 space-y-1.5 bg-slate-900/40 p-2.5 rounded-xl border border-slate-900">
                                    <div className="flex justify-between items-center text-[8px] font-bold text-slate-500 uppercase tracking-wide">
                                      <span>Latest Note Log</span>
                                      <span className="font-mono">{conNotes[0].date}</span>
                                    </div>
                                    <p className="italic text-slate-200 leading-relaxed text-[11px] line-clamp-3">"{conNotes[0].content}"</p>
                                    <span className="block text-[8px] text-slate-500 text-right font-mono">— {conNotes[0].author}</span>
                                  </div>
                                ) : (
                                  <div className="pt-3 border-t border-slate-800">
                                    <p className="text-[10px] text-slate-500 italic">No notes or historical logs recorded for this stakeholder.</p>
                                  </div>
                                )}
                              </div>
                            </div>
                            <p className="text-xs text-slate-500 truncate">{con.role} • <span className="font-semibold text-slate-700">{con.company}</span></p>
                          </div>
                        </div>

                        <div className="mt-4 pt-4 border-t border-slate-100 space-y-2 text-xs">
                          <button
                            onClick={(e) => { e.stopPropagation(); showToast(`Filing outreach email to ${con.email}`, "info"); }}
                            className="flex items-center gap-2 text-slate-500 hover:text-blue-600 transition text-left w-full overflow-hidden"
                          >
                            <Mail className="w-3.5 h-3.5 text-slate-400 shrink-0" />
                            <span className="truncate">{con.email}</span>
                          </button>
                          <button
                            onClick={(e) => { e.stopPropagation(); showToast(`Triggering desk voice system dials to ${con.phone}`, "info"); }}
                            className="flex items-center gap-2 text-slate-500 hover:text-blue-600 transition text-left w-full"
                          >
                            <Phone className="w-3.5 h-3.5 text-slate-400 shrink-0" />
                            <span>{con.phone || "+1 (555) 000-0000"}</span>
                          </button>
                        </div>
                      </div>

                      <div className="mt-4 pt-3 border-t border-slate-100 flex justify-between items-center text-[10px] text-slate-400 font-bold">
                        <span className="flex items-center gap-2">
                          📝 {conNotes.length} notes | 📎 {conDocs.length} attachments
                        </span>
                        <span className="text-blue-600 hover:underline">Link Attributes →</span>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}

          {/* TAB 4: CORPORATE ENTITIES METRICS */}
          {activeTab === "entities" && (
            <div className="space-y-6 animate-in fade-in duration-300">
              <div className="flex justify-between items-center border-b border-slate-200 pb-3">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 tracking-tight">Corporate Organizations</h2>
                  <p className="text-xs text-slate-400">Classified client registry, tier allocations, and workspace linked diagnostics</p>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                {visibleEntities.map((ent) => {
                  const linkedInt = interactions.filter((i) => i.client === ent.name);
                  const entNotes = notes.filter((n) => n.linkedToType === "entity" && n.linkedToId === ent.id);
                  const entDocs = documents.filter((d) => d.linkedToType === "entity" && d.linkedToId === ent.id);

                  return (
                    <div
                      key={ent.id}
                      onClick={() => setSelectedItem({ dataType: "entity", data: ent })}
                      className="bg-white border border-slate-200 rounded-xl p-5 hover:border-purple-300 cursor-pointer shadow-sm transition flex flex-col justify-between"
                    >
                      <div className="space-y-3">
                        <div className="flex justify-between items-start">
                          <div>
                            <h3 className="font-extrabold text-slate-900 text-sm tracking-tight">{ent.name}</h3>
                            <span className="text-[10px] font-bold text-purple-600 uppercase tracking-widest">{ent.industry}</span>
                          </div>
                          <span className="px-2 py-0.5 bg-purple-50 text-purple-700 border border-purple-100 rounded text-[9px] font-bold uppercase tracking-wide">
                            {ent.tier} Account
                          </span>
                        </div>

                        <div className="pt-2 flex items-center gap-1 text-xs text-slate-500 font-semibold">
                          <MapPin className="w-4 h-4 text-slate-400 shrink-0" />
                          <span>HQ Headquarters: <span className="font-medium text-slate-800">{ent.location}</span></span>
                        </div>

                        <div className="bg-slate-50 p-3 rounded-lg flex justify-between items-center text-xs font-semibold">
                          <div className="flex flex-col gap-0.5">
                            <span className="text-slate-400 text-[10px] font-bold">Interactions Registry</span>
                            <span className="text-slate-700 font-mono">{linkedInt.length} Touchpoint Logs</span>
                          </div>
                          <div className="flex flex-col gap-0.5 text-right">
                            <span className="text-slate-400 text-[10px] font-bold font-sans">Active Link Vault</span>
                            <span className="text-slate-900 font-bold font-mono">📝 {entNotes.length} | 📎 {entDocs.length} Docs</span>
                          </div>
                        </div>
                      </div>

                      <div className="mt-4 pt-3 border-t border-slate-100 flex justify-between items-center text-[10px] text-slate-400 font-bold">
                        <span>Authorized Operational Client Profile</span>
                        <span className="text-blue-600 hover:underline">Configure Connections →</span>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}

          {/* TAB 5: CORPORATE ENGAGEMENTS LISTING */}
          {activeTab === "engagements" && (
            <div className="space-y-6 animate-in fade-in duration-300">
              <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 border-b border-slate-200 pb-3">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 tracking-tight flex items-center gap-2">
                    <Handshake className="w-5 h-5 text-sky-500" /> Corporate Engagements
                  </h2>
                  <p className="text-xs text-slate-400 mt-1">High-level strategic agreements, marketing campaigns, SOWs, and contract initiatives</p>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-xs font-bold text-slate-500 bg-slate-100 px-3 py-1.5 rounded-lg border border-slate-200">
                    SOW Budget Allocated: <strong className="text-slate-900 font-mono">${
                      engagements.reduce((acc, curr) => {
                        const parsed = parseInt(curr.budget.replace(/[^0-9]/g, "")) || 0;
                        return acc + parsed;
                      }, 0).toLocaleString()
                    }</strong>
                  </span>
                  <button
                    onClick={() => { setNewType("engagement"); setIsNewModalOpen(true); }}
                    className="p-2 py-1.5 bg-sky-600 hover:bg-sky-700 text-white rounded-lg text-xs font-bold flex items-center gap-1.5 shadow-sm transition duration-205 cursor-pointer border border-transparent outline-none"
                  >
                    <Plus className="w-4 h-4" /> Log Engagement
                  </button>
                </div>
              </div>

              {/* ENGAGEMENT LIFECYCLE TIMELINE COMPONENT */}
              {(() => {
                const todayStr = "2026-06-07";
                const todayDate = new Date(todayStr);

                // Filter based on selected timeline filter: Active-only vs All non-closed
                const itemsToMap = engagements.filter((e) => {
                  if (timelineFilterMode === "Active") {
                    return e.status === "Active";
                  }
                  return e.status !== "Closed"; // Show Active, Under Negotiation, and Pending Draft
                });

                // Compute bounding timeline dates based on selected Zoom mode
                const oneDay = 24 * 60 * 60 * 1000;
                let minTime = todayDate.getTime() - 45 * oneDay;
                let maxTime = todayDate.getTime() + 180 * oneDay;

                if (timelineZoom === "Monthly") {
                  minTime = todayDate.getTime() - 20 * oneDay; // 20 days back
                  maxTime = todayDate.getTime() + 40 * oneDay; // 40 days out
                } else if (timelineZoom === "Quarterly") {
                  minTime = todayDate.getTime() - 60 * oneDay; // 60 days back
                  maxTime = todayDate.getTime() + 120 * oneDay; // 120 days out
                } else if (timelineZoom === "Annual") {
                  minTime = todayDate.getTime() - 150 * oneDay; // 150 days back
                  maxTime = todayDate.getTime() + 300 * oneDay; // 300 days out
                }

                const totalSpan = maxTime - minTime;
                const todayPercent = ((todayDate.getTime() - minTime) / totalSpan) * 100;

                // Month tick calculations (Month start and mid-month)
                const startYearMonth = new Date(minTime);
                const monthMarkers: { label: string; percent: number; isMain: boolean }[] = [];
                let currDate = new Date(startYearMonth.getFullYear(), startYearMonth.getMonth(), 1);

                while (currDate.getTime() <= maxTime + 31 * oneDay) {
                  // Standard month start marker
                  if (currDate.getTime() >= minTime && currDate.getTime() <= maxTime) {
                    const markerPercent = ((currDate.getTime() - minTime) / totalSpan) * 100;
                    monthMarkers.push({
                      label: currDate.toLocaleDateString("en-US", { month: "short", year: "2-digit" }).toUpperCase(),
                      percent: markerPercent,
                      isMain: true
                    });
                  }

                  // Mid-month marker for higher detail in Monthly zoom level
                  if (timelineZoom === "Monthly") {
                    const midDate = new Date(currDate.getFullYear(), currDate.getMonth(), 15);
                    if (midDate.getTime() >= minTime && midDate.getTime() <= maxTime) {
                      const midPercent = ((midDate.getTime() - minTime) / totalSpan) * 100;
                      monthMarkers.push({
                        label: "15TH",
                        percent: midPercent,
                        isMain: false
                      });
                    }
                  }

                  currDate.setMonth(currDate.getMonth() + 1);
                }

                // Sort tickers to keep them sorted in DOM
                monthMarkers.sort((a, b) => a.percent - b.percent);

                return (
                  <div className="bg-slate-900 text-slate-100 p-5 rounded-2xl border border-slate-800 shadow-md flex flex-col gap-4 animate-in fade-in duration-300">
                    <div className="flex flex-col lg:flex-row items-start lg:items-center justify-between gap-4 border-b border-slate-800 pb-3">
                      <div>
                        <div className="flex items-center gap-2">
                          <span className="flex h-2 w-2 relative">
                            <span className="animate-ping absolute inline-flex h-full w-full rounded-full bg-sky-400 opacity-75"></span>
                            <span className="relative inline-flex rounded-full h-2 w-2 bg-sky-500"></span>
                          </span>
                          <h3 className="text-sm font-black tracking-wider uppercase font-mono text-sky-450">Active Project Lifecycles</h3>
                        </div>
                        <p className="text-[10.5px] text-slate-400 font-semibold leading-none mt-1">
                          Visual start-to-end timeline relative to Today • <strong className="text-white">June 7, 2026</strong>
                        </p>
                      </div>

                      {/* Controls container containing both filter options and zoom factors */}
                      <div className="flex flex-wrap items-center gap-3 w-full lg:w-auto">
                        {/* Scope Filter */}
                        <div className="flex items-center gap-1.5 bg-slate-950/40 px-2 py-1 rounded-xl border border-slate-800/60 shadow-sm">
                          <span className="text-[9px] text-slate-500 font-bold uppercase font-mono tracking-wider">Scope:</span>
                          <div className="flex items-center bg-slate-950 p-0.5 rounded-lg border border-slate-850 shadow-inner">
                            <button
                              onClick={() => setTimelineFilterMode("Active")}
                              className={`px-2.5 py-1 rounded text-[10.5px] font-bold transition-all duration-200 ${
                                timelineFilterMode === "Active"
                                  ? "bg-slate-800 text-sky-400 border border-slate-700/60"
                                  : "text-slate-500 hover:text-slate-350"
                              }`}
                            >
                              Active Only
                            </button>
                            <button
                              onClick={() => setTimelineFilterMode("All")}
                              className={`px-2.5 py-1 rounded text-[10.5px] font-bold transition-all duration-200 ${
                                timelineFilterMode === "All"
                                  ? "bg-slate-800 text-sky-400 border border-slate-700/60"
                                  : "text-slate-500 hover:text-slate-350"
                              }`}
                            >
                              All Prospects
                            </button>
                          </div>
                        </div>

                        {/* Zoom Scale */}
                        <div className="flex items-center gap-1.5 bg-slate-950/40 px-2 py-1 rounded-xl border border-slate-800/60 shadow-sm col-span-2">
                          <span className="text-[9px] text-slate-500 font-bold uppercase font-mono tracking-wider">Scale:</span>
                          <div className="flex items-center bg-slate-950 p-0.5 rounded-lg border border-slate-850 shadow-inner">
                            <button
                              onClick={() => setTimelineZoom("Monthly")}
                              className={`px-2.5 py-1 rounded text-[10.5px] font-bold transition-all duration-200 ${
                                timelineZoom === "Monthly"
                                  ? "bg-slate-800 text-sky-400 border border-slate-700/60"
                                  : "text-slate-500 hover:text-slate-350"
                              }`}
                            >
                              Monthly
                            </button>
                            <button
                              onClick={() => setTimelineZoom("Quarterly")}
                              className={`px-2.5 py-1 rounded text-[10.5px] font-bold transition-all duration-200 ${
                                timelineZoom === "Quarterly"
                                  ? "bg-slate-800 text-sky-400 border border-slate-700/60"
                                  : "text-slate-500 hover:text-slate-350"
                              }`}
                            >
                              Quarterly
                            </button>
                            <button
                              onClick={() => setTimelineZoom("Annual")}
                              className={`px-2.5 py-1 rounded text-[10.5px] font-bold transition-all duration-200 ${
                                timelineZoom === "Annual"
                                  ? "bg-slate-800 text-sky-400 border border-slate-700/60"
                                  : "text-slate-500 hover:text-slate-350"
                              }`}
                            >
                              Annual
                            </button>
                          </div>
                        </div>
                      </div>
                    </div>

                    {/* Timeline visualization canvas container */}
                    <div className="relative border border-slate-800/80 bg-slate-955 rounded-xl p-4 overflow-x-auto min-w-0">
                      {/* Grid representation */}
                      <div className="relative min-w-[700px] select-none py-1.5 overflow-visible">

                        {/* Month Vertical Grid Indicators & Labels */}
                        <div className="h-5 relative mb-4 border-b border-slate-850/60 overflow-visible text-slate-500 text-[9px] font-bold font-mono">
                          {monthMarkers.map((marker, idx) => (
                            <div
                              key={idx}
                              className={`absolute -translate-x-1/2 flex flex-col items-center ${
                                marker.isMain ? "text-slate-400" : "text-slate-600 font-medium"
                              }`}
                              style={{ left: `${marker.percent}%` }}
                            >
                              <span>{marker.label}</span>
                              <div className={`w-px h-2 mt-1 ${marker.isMain ? "bg-slate-800" : "bg-slate-900"}`} />
                            </div>
                          ))}
                        </div>

                        {/* Background Grid Vertical Lines */}
                        <div className="absolute inset-x-0 bottom-0 top-10 pointer-events-none overflow-visible z-0">
                          {monthMarkers.map((marker, idx) => (
                            <div
                              key={`line-${idx}`}
                              className={`absolute w-px ${
                                marker.isMain
                                  ? "bg-slate-900 border-l border-dashed border-slate-900/45"
                                  : "bg-slate-950/20 border-l border-dotted border-slate-950/20"
                              }`}
                              style={{ left: `${marker.percent}%`, top: 0, bottom: 0 }}
                            />
                          ))}
                        </div>

                        {/* TODAY CURRENT DATE VERTICAL INDICATOR LINE */}
                        {todayPercent >= 0 && todayPercent <= 100 && (
                          <div
                            className="absolute bottom-0 top-6 w-0.5 pointer-events-none z-30 transition-all duration-350 overflow-visible"
                            style={{ left: `${todayPercent}%` }}
                          >
                            <div className="absolute -top-6 -translate-x-1/2 bg-red-650/90 text-white font-black text-[7.5px] px-1.5 py-0.5 rounded uppercase tracking-wider shadow border border-red-500/30 whitespace-nowrap">
                              Today <span className="font-mono">(Jun 7)</span>
                            </div>
                            <div className="h-full w-[2px] bg-red-500 shadow-[0_0_10px_rgba(239,68,68,0.8)]" />
                          </div>
                        )}

                        {/* BAR rows of engagements */}
                        <div className="space-y-3.5 relative z-10 py-1.5 min-h-[40px] overflow-visible">
                          {itemsToMap.length === 0 ? (
                            <div className="text-center py-8 text-slate-500 italic text-[11px] border border-dashed border-slate-800 rounded-lg">
                              No engagements currently matching this criteria to display on the lifecycle chart.
                            </div>
                          ) : (
                            (() => {
                              const renderedRows = itemsToMap.map((eng) => {
                                const start = new Date(eng.startDate).getTime();
                                const end = new Date(eng.endDate).getTime();
                                const totalDuration = end - start;

                                // Check if completely outside our selected zoom window range
                                const isOutLeft = end < minTime;
                                const isOutRight = start > maxTime;

                                if (isOutLeft || isOutRight) {
                                  return null;
                                }

                                const startClamped = Math.max(minTime, start);
                                const endClamped = Math.min(maxTime, end);

                                const leftPercent = ((startClamped - minTime) / totalSpan) * 100;
                                const rightPercent = ((endClamped - minTime) / totalSpan) * 100;
                                const widthPercent = Math.max(4, rightPercent - leftPercent);

                                const isClippedLeft = start < minTime;
                                const isClippedRight = end > maxTime;

                                // Calculate age progress percentage relative to today
                                let elapsedPercent = 0;
                                if (todayDate.getTime() > start) {
                                  if (todayDate.getTime() >= end) {
                                    elapsedPercent = 100;
                                  } else {
                                    elapsedPercent = Math.round(((todayDate.getTime() - start) / totalDuration) * 100);
                                  }
                                }

                                const totalDays = Math.round(totalDuration / (1000 * 24 * 60 * 60));
                                const elapsedDays = todayDate.getTime() > start
                                  ? Math.min(totalDays, Math.round((todayDate.getTime() - start) / (1000 * 24 * 60 * 60)))
                                  : 0;
                                const daysRemaining = Math.max(0, totalDays - elapsedDays);

                                // Status-specific visuals
                                const styleMap = {
                                  "Active": { barBg: "from-emerald-500/10 to-emerald-400/20 border-emerald-500/40 text-emerald-300", pinColor: "bg-emerald-400" },
                                  "Under Negotiation": { barBg: "from-amber-500/10 to-amber-400/15 border-amber-500/30 text-amber-350", pinColor: "bg-amber-450" },
                                  "Pending Draft": { barBg: "from-sky-500/10 to-sky-400/15 border-sky-500/30 text-sky-350", pinColor: "bg-sky-450" },
                                  "Closed": { barBg: "from-slate-500/5 to-slate-400/10 border-slate-500/20 text-slate-400", pinColor: "bg-slate-400" }
                                }[eng.status] || { barBg: "from-sky-500/10 to-sky-400/15 border-sky-500/30 text-sky-350", pinColor: "bg-sky-400" };

                                return (
                                  <div
                                    key={eng.id}
                                    className="relative group/timeline-row"
                                    onClick={() => setSelectedItem({ dataType: "engagement", data: eng })}
                                  >
                                    {/* Visual Bar Track */}
                                    <div
                                      className="relative h-10 flex items-center bg-gradient-to-r shadow-inner group-hover/timeline-row:brightness-110 cursor-pointer transition-all duration-200"
                                      style={{
                                        left: `${leftPercent}%`,
                                        width: `${widthPercent}%`,
                                      }}
                                    >
                                      <div className={`p-1.5 h-full w-full border ${
                                        isClippedLeft ? "rounded-l-none border-l-dashed border-l-2" : "rounded-l-xl"
                                      } ${
                                        isClippedRight ? "rounded-r-none border-r-dashed border-r-2" : "rounded-r-xl"
                                      } ${styleMap.barBg} relative overflow-hidden flex flex-col justify-center`}>

                                        {/* Shaded elapsed indicator background inside the bar */}
                                        {elapsedPercent > 0 && (
                                          <div
                                            className="absolute left-0 top-0 bottom-0 bg-slate-100/[0.04] transition-all duration-305 bg-gradient-to-r from-sky-500/5 to-sky-400/10"
                                            style={{ width: `${elapsedPercent}%` }}
                                          />
                                        )}

                                        {/* Project name text and client inside bar */}
                                        <div className="relative z-10 px-1 truncate select-none leading-none flex items-center gap-1.5">
                                          {isClippedLeft && <span className="text-[10px] text-slate-400 font-extrabold select-none animate-pulse">«</span>}
                                          <span className={`w-1.5 h-1.5 rounded-full ${styleMap.pinColor} shrink-0`} />
                                          <span className="font-extrabold text-[10.5px] tracking-tight">{eng.title}</span>
                                          <span className="text-slate-500 font-medium text-[9px] truncate">• {eng.client}</span>
                                          {isClippedRight && <span className="text-[10px] text-slate-400 font-extrabold select-none animate-pulse">»</span>}
                                        </div>

                                        {/* Dates inline sub-row */}
                                        <div className="relative z-10 px-1 mt-1 font-mono text-[8.5px] font-bold text-slate-405/90 leading-none truncate select-none">
                                          {eng.startDate} to {eng.endDate} • <span className="text-slate-500">{totalDays} days</span>
                                        </div>
                                      </div>

                                      {/* INTERACTIVE FLYOUT OVERLAY CARD ON ROW HOVER */}
                                      <div className="absolute top-1/2 -translate-y-1/2 left-full ml-3 w-80 p-5 bg-slate-950 text-slate-100 border border-slate-800 shadow-[0_20px_50px_rgba(0,0,0,0.85)] rounded-2xl invisible opacity-0 scale-95 group-hover/timeline-row:visible group-hover/timeline-row:opacity-100 group-hover/timeline-row:scale-100 transition-all duration-250 z-[95] pointer-events-none text-xs space-y-3.5">
                                        <div className="space-y-1">
                                          <div className="flex items-center justify-between">
                                            <span className="text-[8px] font-extrabold font-mono text-sky-400 tracking-wider">PROJECT TIMELINE</span>
                                            <span className={`px-2 py-0.2 border rounded text-[7px] font-black uppercase ${
                                              eng.status === 'Active' ? 'bg-emerald-950/50 text-emerald-400 border-emerald-800/40' : 'bg-slate-900/50 text-slate-350 border-slate-800/40'
                                            }`}>
                                              {eng.status}
                                            </span>
                                          </div>
                                          <h4 className="text-white font-extrabold text-sm tracking-tight leading-snug whitespace-normal">{eng.title}</h4>
                                          <p className="text-slate-400 font-semibold text-[10.5px]">Client • <span className="font-extrabold text-white">{eng.client}</span></p>
                                        </div>

                                        {/* Graphical Range details */}
                                        <div className="pt-2.5 border-t border-slate-850 text-[10.5px] space-y-2">
                                          <div className="flex justify-between font-mono text-[9.5px]">
                                            <div>
                                              <span className="text-slate-500 font-bold uppercase text-[7.5px] block font-sans">Start Term</span>
                                              <strong className={isClippedLeft ? "text-amber-400" : "text-sky-405"}>
                                                {eng.startDate} {isClippedLeft && "(Beyond Visual range)"}
                                              </strong>
                                            </div>
                                            <div className="text-right">
                                              <span className="text-slate-500 font-bold uppercase text-[7.5px] block font-sans">End Term</span>
                                              <strong className={isClippedRight ? "text-amber-400" : "text-emerald-400"}>
                                                {eng.endDate} {isClippedRight && "(Beyond Visual range)"}
                                              </strong>
                                            </div>
                                          </div>

                                          {/* Inline graphic progress bar bar */}
                                          <div className="space-y-1">
                                            <div className="flex justify-between items-center text-[9px] font-bold text-slate-400">
                                              <span>Timeline progress</span>
                                              <span className="text-white">{elapsedPercent}% Elapsed</span>
                                            </div>
                                            <div className="w-full h-1.5 bg-slate-900 rounded-full overflow-hidden border border-slate-850">
                                              <div
                                                className="h-full bg-gradient-to-r from-sky-500 to-sky-400 rounded-full"
                                                style={{ width: `${elapsedPercent}%` }}
                                              />
                                            </div>
                                          </div>

                                          <p className="text-[9.5px] font-mono text-slate-400 leading-none">
                                            ⏱️ <strong>{elapsedDays} days</strong> processed • <strong>{daysRemaining} days</strong> remaining in contract lifecycle.
                                          </p>
                                        </div>

                                        {/* Budget highlight */}
                                        <div className="pt-2.5 border-t border-slate-850 flex items-center justify-between text-[10px]">
                                          <span className="text-slate-500 font-bold uppercase text-[7.5px] tracking-wider font-mono">Contract valuation</span>
                                          <span className="text-emerald-400 font-bold font-mono text-xs">{eng.budget}</span>
                                        </div>
                                      </div>
                                    </div>
                                  </div>
                                );
                              }).filter(Boolean);

                              if (renderedRows.length === 0) {
                                return (
                                  <div className="text-center py-8 text-slate-500 italic text-[11px] border border-dashed border-slate-800 rounded-lg">
                                    No active engagements span across the selected zoom scale list ({timelineZoom}). Try altering the Scale/Scope.
                                  </div>
                                );
                              }

                              return renderedRows;
                            })()
                          )}
                        </div>

                      </div>
                    </div>
                  </div>
                );
              })()}

              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {engagements.map((eng) => {
                  const engNotes = notes.filter((n) => n.linkedToType === "engagement" && n.linkedToId === eng.id);
                  const engDocs = documents.filter((d) => d.linkedToType === "engagement" && d.linkedToId === eng.id);

                  const statusColors = {
                    "Active": "bg-emerald-50 text-emerald-700 border-emerald-100",
                    "Under Negotiation": "bg-amber-50 text-amber-700 border-amber-100",
                    "Pending Draft": "bg-blue-50 text-blue-700 border-blue-100",
                    "Closed": "bg-slate-100 text-slate-500 border-slate-200"
                  }[eng.status] || "bg-slate-50 text-slate-600 border-slate-200";

                  // Calculate remaining budget status
                  const numericBudget = parseInt(eng.budget.replace(/[^0-9]/g, "")) || 0;
                  const getRemainingBudgetStatus = () => {
                    if (numericBudget === 0) {
                      return {
                        percent: 100,
                        amount: 0,
                        label: "No Budget Disclosed",
                        badge: "N/A",
                        badgeColor: "bg-slate-500/20 text-slate-400 border-slate-800/40"
                      };
                    }
                    if (eng.status === "Closed") {
                      return {
                        percent: 0,
                        amount: 0,
                        label: "$0 remaining (100% fully consumed)",
                        badge: "Settle Complete",
                        badgeColor: "bg-red-500/20 text-red-400 border-red-900/40"
                      };
                    }
                    if (eng.status === "Pending Draft" || eng.status === "Under Negotiation") {
                      return {
                        percent: 100,
                        amount: numericBudget,
                        label: `$${numericBudget.toLocaleString()} remaining (100% allocated)`,
                        badge: "Unreleased",
                        badgeColor: "bg-blue-500/20 text-blue-400 border-blue-900/40"
                      };
                    }
                    // Active items: dynamic drawdown balance based on linked notes & docs
                    const drawDownPercent = Math.min(85, Math.max(15, (engNotes.length * 15) || 25));
                    const remainingPercent = 100 - drawDownPercent;
                    const remainingAmount = Math.round(numericBudget * (remainingPercent / 100));
                    return {
                      percent: remainingPercent,
                      amount: remainingAmount,
                      label: `$${remainingAmount.toLocaleString()} remaining (${remainingPercent}% dynamic reserve)`,
                      badge: remainingPercent > 50 ? "Healthy Draw" : "Limited Funds",
                      badgeColor: remainingPercent > 50 ? "bg-emerald-500/20 text-emerald-400 border-emerald-950/40" : "bg-amber-500/20 text-amber-400 border-amber-950/40"
                    };
                  };
                  const budgetDetails = getRemainingBudgetStatus();

                  const cardBorderLeftColor = {
                    "Active": "border-l-emerald-500",
                    "Under Negotiation": "border-l-amber-500",
                    "Pending Draft": "border-l-blue-500",
                    "Closed": "border-l-slate-400"
                  }[eng.status] || "border-l-slate-300";

                  return (
                    <div
                      key={eng.id}
                      onClick={() => setSelectedItem({ dataType: "engagement", data: eng })}
                      className={`bg-white border border-slate-200 border-l-4 ${cardBorderLeftColor} rounded-xl p-5 hover:border-sky-450 cursor-pointer shadow-sm hover:shadow-md transition duration-200 flex flex-col justify-between`}
                    >
                      <div className="space-y-3.5">
                        <div className="flex justify-between items-start gap-2">
                          <div className="relative min-w-0 flex-1">
                            {/* Engagement Title Hover Wrapper */}
                            <div className="relative group/engtitle inline-block max-w-full">
                              <h3 className="font-extrabold text-slate-900 text-sm tracking-tight truncate hover:text-sky-600 transition flex items-center gap-1 cursor-help" title={eng.title}>
                                {eng.title}
                                <span className="inline-block w-1.5 h-1.5 rounded-full bg-sky-500 shrink-0 animate-pulse" />
                              </h3>

                              {/* FLOATING PREVIEW CARD ON HOVER */}
                              <div className="absolute invisible opacity-0 scale-95 origin-top-left translate-y-2 group-hover/engtitle:visible group-hover/engtitle:opacity-100 group-hover/engtitle:scale-100 group-hover/engtitle:translate-y-0 transition-all duration-300 z-[90] left-0 top-full mt-2 w-80 p-5 bg-slate-950 text-slate-100 border border-slate-800 shadow-[0_25px_60px_-15px_rgba(0,0,0,0.8)] rounded-2xl cursor-default pointer-events-none text-xs space-y-4">
                                <div className="space-y-1.5">
                                  <div className="flex items-center justify-between">
                                    <span className="text-[9px] font-bold text-sky-400 uppercase tracking-widest font-mono">Engagement Summary</span>
                                    <span className={`px-2 py-0.5 border rounded text-[8px] font-bold uppercase tracking-wider ${
                                      eng.status === 'Active' ? 'bg-emerald-950/40 text-emerald-400 border-emerald-800/40' : 'bg-slate-900/40 text-slate-300 border-slate-800/40'
                                    }`}>
                                      {eng.status}
                                    </span>
                                  </div>
                                  <h4 className="text-white font-extrabold text-sm tracking-tight leading-snug whitespace-normal">{eng.title}</h4>
                                  <p className="text-slate-400 font-semibold text-[10.5px]">Client • <span className="font-extrabold text-white">{eng.client}</span></p>
                                </div>

                                {/* SOW Description */}
                                <div className="pt-3 border-t border-slate-800/80 space-y-1">
                                  <span className="text-[9px] font-bold text-slate-500 uppercase tracking-widest font-mono block">SOW Scope of Work</span>
                                  <p className="text-slate-200 leading-relaxed text-[11px] font-medium italic whitespace-normal">
                                    "{eng.description || 'No detailed scope of work specified.'}"
                                  </p>
                                </div>

                                {/* Start Date */}
                                <div className="pt-3 border-t border-slate-800/80 grid grid-cols-2 gap-2 text-[10px]">
                                  <div className="flex flex-col bg-slate-900 p-2 rounded border border-slate-850">
                                    <span className="text-slate-500 font-bold uppercase text-[7px] tracking-wider">Start Date</span>
                                    <span className="font-bold text-sky-455 mt-0.5 font-mono">{eng.startDate}</span>
                                  </div>
                                  <div className="flex flex-col bg-slate-900 p-2 rounded border border-slate-850">
                                    <span className="text-slate-500 font-bold uppercase text-[7px] tracking-wider">Agreement Type</span>
                                    <span className="font-semibold text-slate-200 truncate mt-0.5">{eng.type}</span>
                                  </div>
                                </div>

                                {/* Remaining Budget Status */}
                                <div className="pt-3 border-t border-slate-800/80 space-y-2">
                                  <div className="flex items-center justify-between">
                                    <span className="text-[9px] font-bold text-slate-500 uppercase tracking-widest font-mono">Remaining Budget Status</span>
                                    <span className={`px-1.5 py-0.5 border rounded text-[7px] font-extrabold uppercase ${budgetDetails.badgeColor}`}>
                                      {budgetDetails.badge}
                                    </span>
                                  </div>
                                  <div className="space-y-1.5">
                                    <div className="flex justify-between items-center text-[10px]">
                                      <span className="text-slate-400 font-semibold">Total: <strong className="text-white">{eng.budget}</strong></span>
                                      <span className="text-emerald-400 font-extrabold">{budgetDetails.percent}% Reserve</span>
                                    </div>
                                    <div className="w-full h-1.5 bg-slate-900 rounded-full overflow-hidden border border-slate-850">
                                      <div
                                        className={`h-full rounded-full transition-all duration-500 ${
                                          budgetDetails.percent > 50 ? 'bg-emerald-500' : 'bg-amber-500'
                                        }`}
                                        style={{ width: `${budgetDetails.percent}%` }}
                                      />
                                    </div>
                                    <p className="text-[9.5px] font-mono text-slate-400 italic">
                                      {budgetDetails.label}
                                    </p>
                                  </div>
                                </div>
                              </div>
                            </div>

                            <button
                              type="button"
                              onClick={(e) => { e.stopPropagation(); setSearchQuery(eng.client); }}
                              className="text-[11px] font-bold text-slate-500 hover:text-sky-600 truncate block mt-0.5"
                            >
                              🏢 {eng.client}
                            </button>
                          </div>
                          <span className={`px-2 py-0.5 rounded text-[8px] font-extrabold uppercase shrink-0 border ${statusColors}`}>
                            {eng.status}
                          </span>
                        </div>

                        <p className="text-[11px] text-slate-500 leading-relaxed font-normal min-h-[48px] line-clamp-3">
                          {eng.description}
                        </p>

                        <div className="grid grid-cols-2 gap-3 bg-slate-50 p-2.5 rounded-lg border border-slate-100/60 font-mono text-[10px] text-slate-500">
                          <div>
                            <span className="block text-[8px] font-sans font-bold text-slate-400 uppercase">Valuation</span>
                            <span className="font-bold text-slate-800 text-[11px]">{eng.budget}</span>
                          </div>
                          <div>
                            <span className="block text-[8px] font-sans font-bold text-slate-400 uppercase">Agreement Type</span>
                            <span className="font-medium text-slate-600 text-[11px] truncate block" title={eng.type}>{eng.type}</span>
                          </div>
                          <div className="col-span-2 pt-1 border-t border-slate-100/80 flex justify-between font-sans text-slate-400">
                            <span>SOW Term:</span>
                            <strong className="text-slate-800 font-mono text-[9px]">{eng.startDate} to {eng.endDate}</strong>
                          </div>
                        </div>
                      </div>

                      <div className="mt-4 pt-3 border-t border-slate-100 flex justify-between items-center text-[10px] text-slate-400 font-bold font-sans">
                        <span className="flex items-center gap-1.5 text-slate-400">
                          📝 {engNotes.length} notes • 📎 {engDocs.length} attachments
                        </span>
                        <span className="text-sky-600 hover:underline">Manage Engagement →</span>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}

          {/* TAB 6: CENTRAL NOTES LEDGER LINK VIEW */}
          {activeTab === "notes" && (
            <div className="space-y-6 animate-in fade-in duration-300">
              <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 border-b border-slate-200 pb-3">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 tracking-tight flex items-center gap-2">
                    <Notebook className="w-5 h-5 text-indigo-500" /> Active Notes Ledger
                  </h2>
                  <p className="text-xs text-slate-400 mt-1">Centralized operational notes mapped to contracts, clients, partners, and touchpoints</p>
                </div>
                <div className="flex gap-2">
                  <input
                    type="text"
                    placeholder="Search note files..."
                    onChange={(e) => setSearchQuery(e.target.value)}
                    className="bg-white border border-slate-200 rounded-lg px-3 py-1.5 text-xs outline-none focus:border-indigo-500 transition"
                  />
                  <button
                    onClick={() => {
                      const content = prompt("Enter General Note Content:");
                      if (content) {
                        const newNoteObj: Note = {
                          id: `note-${Date.now()}`,
                          content,
                          createdAt: new Date().toISOString(),
                          author: session?.name || "System Operator",
                          linkedToType: "document",
                          linkedToId: "general",
                          pinned: 0
                        };
                        setNotes([newNoteObj, ...notes]);
                        showToast("General note logged to Workspace Vault.", "success");
                        syncToServer("/api/notes", "POST", newNoteObj);
                      }
                    }}
                    className="p-2 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-xs font-bold flex items-center gap-1.5 shadow-sm transition cursor-pointer"
                  >
                    <Plus className="w-4 h-4" /> Add Note
                  </button>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {filteredAndSortedNotes.map((note) => {
                  let linkLabel = "General Workspace Note";
                  let linkBadgeColor = "bg-slate-100 text-slate-600";
                  if (note.linkedToType === "interaction") {
                    const found = interactions.find((i) => i.id === note.linkedToId);
                    linkLabel = found ? `Meeting: ${found.subject}` : "Linked Interaction";
                    linkBadgeColor = "bg-amber-50 text-amber-700 border-amber-100";
                  } else if (note.linkedToType === "contact") {
                    const found = contacts.find((c) => c.id === note.linkedToId);
                    linkLabel = found ? `Contact: ${found.name}` : "Linked Contact";
                    linkBadgeColor = "bg-emerald-50 text-emerald-700 border-emerald-100";
                  } else if (note.linkedToType === "entity") {
                    const found = entities.find((e) => e.id === note.linkedToId);
                    linkLabel = found ? `Entity: ${found.name}` : "Linked Corporate Entity";
                    linkBadgeColor = "bg-purple-50 text-purple-700 border-purple-100";
                  } else if (note.linkedToType === "engagement") {
                    const found = engagements.find((g) => g.id === note.linkedToId);
                    linkLabel = found ? `Engagement: ${found.title}` : "Linked Engagement";
                    linkBadgeColor = "bg-sky-50 text-sky-700 border-sky-100";
                  }

                  return (
                    <div
                      key={note.id}
                      className={`bg-white border rounded-xl p-5 shadow-sm hover:shadow transition duration-200 flex flex-col justify-between space-y-4 ${
                        note.pinned
                          ? "border-indigo-300 ring-2 ring-indigo-50/50"
                          : "border-slate-200 hover:border-indigo-300"
                      }`}
                    >
                      <div className="space-y-3">
                        <div className="flex justify-between items-start gap-2">
                          <div className="flex items-center gap-1.5 overflow-hidden">
                            <button
                              onClick={() => handleTogglePinNote(note.id)}
                              className={`p-1 rounded transition cursor-pointer shrink-0 ${
                                note.pinned
                                  ? "text-indigo-600 bg-indigo-50 hover:bg-indigo-100"
                                  : "text-slate-400 hover:text-indigo-600 hover:bg-slate-50"
                              }`}
                              title={note.pinned ? "Unpin Note" : "Pin Note"}
                            >
                              {note.pinned ? (
                                <Pin className="w-3.5 h-3.5 fill-indigo-600" />
                              ) : (
                                <Pin className="w-3.5 h-3.5" />
                              )}
                            </button>
                            <span className={`px-2 py-0.5 rounded text-[8px] font-extrabold uppercase border truncate max-w-[170px] ${linkLabel.startsWith("General") ? "bg-slate-50 border-slate-200 text-slate-550" : "border-" + linkBadgeColor}`}>
                              {linkLabel}
                            </span>
                          </div>
                          <button
                            onClick={() => {
                              if (confirm("Are you sure you want to discard this ledger note?")) {
                                setNotes((prev) => prev.filter((n) => n.id !== note.id));
                                showToast("Note discarded from Workspace ledger.", "info");
                                syncToServer(`/api/notes/${note.id}`, "DELETE");
                              }
                            }}
                            className="p-1 text-slate-400 hover:text-rose-500 rounded hover:bg-rose-50 transition cursor-pointer"
                            title="Discard note file"
                          >
                            <Trash2 className="w-3.5 h-3.5" />
                          </button>
                        </div>

                        <p className="text-xs text-slate-700 font-normal leading-relaxed whitespace-pre-wrap font-sans">
                          {note.content}
                        </p>
                      </div>

                      <div className="pt-3 border-t border-slate-100 flex justify-between items-center text-[10px] text-slate-400 font-mono font-bold">
                        <span>By: {note.author}</span>
                        <span>{new Date(note.createdAt).toLocaleDateString()}</span>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}

          {/* TAB 7: CENTRAL DOCUMENT VAULT LIST VIEW */}
          {activeTab === "documents" && (
            <div className="space-y-6 animate-in fade-in duration-300">
              <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 border-b border-slate-200 pb-3">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 tracking-tight flex items-center gap-2">
                    <Paperclip className="w-5 h-5 text-violet-500" /> Executive Document Vault
                  </h2>
                  <p className="text-xs text-slate-400 mt-1">Secure centralized repository for attached briefing packs, pitch decks, PDFs, and legal contracts</p>
                </div>
                <div className="flex gap-2">
                  <button
                    onClick={() => {
                      const title = prompt("Enter Document Title:");
                      if (title) {
                        const newDocObj: Document = {
                          id: `doc-${Date.now()}`,
                          title,
                          fileType: "PDF",
                          fileSize: "2.4 MB",
                          uploadedAt: new Date().toISOString().split("T")[0],
                          linkedToType: "interaction",
                          linkedToId: "general"
                        };
                        setDocuments([newDocObj, ...documents]);
                        showToast(`Document '${title}' indexed in vault.`, "success");
                      }
                    }}
                    className="p-2 py-1.5 bg-violet-600 hover:bg-violet-700 text-white rounded-lg text-xs font-bold flex items-center gap-1.5 shadow-sm transition cursor-pointer"
                  >
                    <Plus className="w-4 h-4" /> Index Document Link
                  </button>
                </div>
              </div>

              {/* Drag and Drop simulate widget */}
              <div
                onDragOver={(e) => e.preventDefault()}
                onDrop={(e) => {
                  e.preventDefault();
                  showToast("Securing file packets... File parsed successfully!", "success");
                  const newDocObj: Document = {
                    id: `doc-${Date.now()}`,
                    title: "Dragged_Briefing_Package.pdf",
                    fileType: "Briefing",
                    fileSize: "1.8 MB",
                    uploadedAt: new Date().toISOString().split("T")[0],
                    linkedToType: "interaction",
                    linkedToId: "general"
                  };
                  setDocuments([newDocObj, ...documents]);
                }}
                className="border-2 border-dashed border-slate-200 hover:border-violet-300 bg-white hover:bg-slate-50/50 p-6 rounded-2xl transition duration-200 text-center space-y-2 cursor-pointer"
              >
                <div className="w-10 h-10 bg-violet-50 text-violet-600 rounded-full flex items-center justify-center mx-auto">
                  <Paperclip className="w-5 h-5" />
                </div>
                <div className="space-y-1">
                  <p className="text-xs font-bold text-slate-800">Drag & drop files here to upload to the secure vault</p>
                  <p className="text-[10px] text-slate-400">PDFs, Spreadsheets, Presentations, Contracts, or Briefings up to 50MB</p>
                </div>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-4 gap-6">
                {documents.map((doc) => {
                  let nodeLabel = "General Vault Document";
                  let linkColor = "bg-slate-100 text-slate-600";
                  if (doc.linkedToType === "interaction") {
                    const found = interactions.find((i) => i.id === doc.linkedToId);
                    nodeLabel = found ? `Meeting: ${found.subject}` : "Interaction Context";
                    linkColor = "bg-amber-50 text-amber-700 border-amber-100";
                  } else if (doc.linkedToType === "contact") {
                    const found = contacts.find((c) => c.id === doc.linkedToId);
                    nodeLabel = found ? `Contact: ${found.name}` : "Stakeholder Context";
                    linkColor = "bg-emerald-50 text-emerald-700 border-emerald-100";
                  } else if (doc.linkedToType === "entity") {
                    const found = entities.find((e) => e.id === doc.linkedToId);
                    nodeLabel = found ? `Entity: ${found.name}` : "Entity Corporate Context";
                    linkColor = "bg-purple-50 text-purple-700 border-purple-100";
                  } else if (doc.linkedToType === "engagement") {
                    const found = engagements.find((e) => e.id === doc.linkedToId);
                    nodeLabel = found ? `Engagement: ${found.title}` : "Engagement Contract Context";
                    linkColor = "bg-sky-50 text-sky-700 border-sky-100";
                  }

                  return (
                    <div
                      key={doc.id}
                      className="bg-white border border-slate-200 hover:border-violet-300 rounded-xl p-4.5 shadow-sm hover:shadow transition duration-200 flex flex-col justify-between space-y-3"
                    >
                      <div className="space-y-2.5">
                        <div className="flex justify-between items-start gap-2">
                          <span className="px-1.5 py-0.5 bg-violet-50 text-violet-700 font-extrabold font-mono text-[8px] rounded border border-violet-100 uppercase tracking-widest leading-none">
                            {doc.fileType}
                          </span>
                          <button
                            onClick={() => {
                              if (confirm(`Do you wish to discard document registration for ${doc.title}?`)) {
                                setDocuments((prev) => prev.filter((d) => d.id !== doc.id));
                                showToast("Document index entries purged from vault.", "info");
                              }
                            }}
                            className="p-1 text-slate-400 hover:text-rose-500 rounded hover:bg-rose-50 transition cursor-pointer"
                            title="Discard document entry"
                          >
                            <Trash2 className="w-3.5 h-3.5" />
                          </button>
                        </div>

                        <div className="overflow-hidden">
                          <h4 className="font-bold text-slate-900 text-xs truncate leading-snug" title={doc.title}>{doc.title}</h4>
                          <span className="text-[10px] text-slate-400 font-mono mt-0.5 block">{doc.fileSize} • Indexed {doc.uploadedAt}</span>
                        </div>
                      </div>

                      <div className="pt-3 border-t border-slate-100 flex flex-col gap-2">
                        <span className={`px-2 py-0.5 rounded text-[8px] font-extrabold uppercase border truncate text-center block ${nodeLabel.startsWith("General") ? "bg-slate-50 border-slate-200 text-slate-550" : "border-" + linkColor}`}>
                          {nodeLabel}
                        </span>
                        <button
                          onClick={() => showToast(`Successfully retrieved contract packets representing ${doc.title}`, "success")}
                          className="w-full text-center py-1.5 bg-slate-50 hover:bg-violet-50 border border-slate-200 hover:border-violet-200 text-slate-800 hover:text-violet-700 rounded-lg text-[10px] font-bold tracking-wide transition duration-150 cursor-pointer"
                        >
                          Retrieve / Download Original
                        </button>
                      </div>
                    </div>
                  );
                })}
              </div>
            </div>
          )}

          {/* TAB 8: OPERATING AUDITOR LEVEL SEATS */}
          {activeTab === "users" && (
            <div className="space-y-6 animate-in fade-in duration-300">
              <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 border-b border-slate-200 pb-3">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 tracking-tight flex items-center gap-2">
                    <UserCheck className="w-5 h-5 text-pink-500" /> Operators & Seating Control Console
                  </h2>
                  <p className="text-xs text-slate-400 mt-1">Verify logged workspace operators, register new administrative seats, and audit credentials</p>
                </div>
                <button
                  onClick={() => { setNewType("user"); setIsNewModalOpen(true); }}
                  className="p-2 py-1.5 bg-pink-600 hover:bg-pink-700 text-white rounded-lg text-xs font-bold flex items-center gap-1.5 shadow-sm transition cursor-pointer"
                >
                  <Plus className="w-4 h-4" /> Register Auditor Seat
                </button>
              </div>

              <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
                {systemUsers.map((usr) => (
                  <div
                    key={usr.email}
                    className="bg-white border border-slate-200 hover:border-pink-300 rounded-xl p-5 shadow-sm hover:shadow transition duration-200 flex flex-col justify-between"
                  >
                    <div className="space-y-3.5">
                      <div className="flex items-center gap-3">
                        <div className="w-10 h-10 rounded-full bg-pink-50 text-pink-600 flex items-center justify-center font-extrabold text-xs uppercase select-none border border-pink-100 pb-0.5">
                          {usr.name ? usr.name.split(" ").map((n: string) => n[0]).join("") : "OP"}
                        </div>
                        <div className="overflow-hidden leading-none space-y-1">
                          <h4 className="font-extrabold text-slate-900 text-xs truncate leading-normal">{usr.name}</h4>
                          <span className="text-[10px] font-bold text-pink-600 bg-pink-50/50 px-1.5 py-0.5 rounded uppercase tracking-wider">{usr.role}</span>
                        </div>
                      </div>

                      <div className="bg-slate-50 p-3 rounded-lg border border-slate-100 flex flex-col gap-2 font-mono text-[10px] text-slate-500">
                        <div className="flex justify-between">
                          <span>Operator Email:</span>
                          <strong className="text-slate-800 font-sans">{usr.email}</strong>
                        </div>
                        <div className="flex justify-between items-center">
                          <span>Passphrase State:</span>
                          <span className="inline-flex items-center gap-1 text-[9px] text-emerald-600 font-bold bg-emerald-50 px-1.5 py-0.2 rounded border border-emerald-100">
                            Securely Hashed
                          </span>
                        </div>
                      </div>
                    </div>

                    <div className="mt-4 pt-3 border-t border-slate-100 flex justify-between items-center text-xs">
                      <div className="flex items-center gap-1.5">
                        <span className={`w-1.5 h-1.5 rounded-full ${usr.suspended ? "bg-rose-500 animate-pulse" : "bg-emerald-500 animate-pulse"}`}></span>
                        <span className={`text-[10px] font-bold font-mono ${usr.suspended ? "text-rose-500" : "text-emerald-600"}`}>
                          {usr.suspended ? "Suspended" : "Active Seat"}
                        </span>
                      </div>
                      {usr.email.toLowerCase() === session?.email?.toLowerCase() ? (
                        <span className="text-[10px] text-indigo-500 bg-indigo-50/50 font-bold px-2 py-0.5 rounded border border-indigo-100/50">
                          Your Active Session
                        </span>
                      ) : usr.suspended ? (
                        <button
                          onClick={() => handleRestoreUser(usr.email)}
                          className="px-2.5 py-1 text-xs font-bold text-emerald-600 hover:text-white bg-emerald-50 hover:bg-emerald-600 rounded-lg border border-emerald-100 hover:border-emerald-600 transition cursor-pointer"
                        >
                          Reactivate Seat
                        </button>
                      ) : (
                        <button
                          onClick={() => {
                            if (confirm(`Are you sure you want to suspend access seat privileges (soft-delete) for ${usr.name}?`)) {
                              handleDeleteItem("user", usr.email);
                            }
                          }}
                          className="px-2.5 py-1 text-xs font-bold text-rose-600 hover:text-white bg-rose-50 hover:bg-rose-600 rounded-lg border border-rose-100 hover:border-rose-600 transition cursor-pointer"
                        >
                          Suspend Seat
                        </button>
                      )}
                    </div>
                  </div>
                ))}
              </div>
            </div>
          )}

          {/* TAB 9: SYSTEM AUDIT LEDGER */}
          {activeTab === "audit" && (
            <div className="space-y-6 animate-in fade-in duration-300" id="audit-ledger-container">
              {/* Header block */}
              <div className="flex flex-col md:flex-row justify-between items-start md:items-center gap-4 border-b border-slate-200 pb-4">
                <div>
                  <h2 className="text-xl font-bold text-slate-900 tracking-tight flex items-center gap-2">
                    <ShieldCheck className="w-5 h-5 text-amber-500" /> Security Audit Trail & System Lineage
                  </h2>
                  <p className="text-xs text-slate-400 mt-1">
                    Continuous monitoring of workspace operations, credential validations, bulk deletes, and corporate entity mutations.
                  </p>
                </div>
                <div className="flex items-center gap-2.5">
                  <button
                    onClick={() => loadAuditLogs()}
                    className="p-2 py-1.5 bg-slate-100 hover:bg-slate-200 text-slate-700 rounded-lg text-xs font-bold flex items-center gap-1.5 transition border border-slate-200 cursor-pointer"
                    title="Reload ledger from SQLite securely"
                  >
                    <RefreshCw className="w-3.5 h-3.5" /> Reload Ledger
                  </button>
                  <button
                    onClick={() => purgeAuditLogs()}
                    className="p-2 py-1.5 bg-red-50 hover:bg-red-600 text-red-600 hover:text-white rounded-lg text-xs font-bold flex items-center gap-1.5 transition border border-red-200 hover:border-red-600 cursor-pointer shadow-sm"
                    title="Permanently empty all historical security lines"
                  >
                    <Trash2 className="w-4 h-4" /> Purge System Trails
                  </button>
                </div>
              </div>

              {/* Bento statistical metrics grid */}
              <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
                <div className="bg-slate-50 border border-slate-200 p-4 rounded-xl flex items-center gap-4">
                  <div className="p-3 bg-slate-900 text-white rounded-lg">
                    <History className="w-5 h-5 text-slate-300" />
                  </div>
                  <div>
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest block leading-none">Total Logs</span>
                    <strong className="text-xl font-extrabold text-slate-900 leading-none block mt-1">{auditLogs.length}</strong>
                  </div>
                </div>

                <div className="bg-slate-50 border border-slate-200 p-4 rounded-xl flex items-center gap-4">
                  <div className="p-3 bg-emerald-100 text-emerald-800 rounded-lg">
                    <Check className="w-5 h-5 text-emerald-600" />
                  </div>
                  <div>
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest block leading-none">Record Matches</span>
                    <strong className="text-xl font-extrabold text-slate-900 leading-none block mt-1">
                      {auditLogs.filter((log) => {
                        if (auditTargetFilter !== "ALL" && log.targetType !== auditTargetFilter) return false;
                        if (auditActionFilter !== "ALL") {
                          if (auditActionFilter === "BATCH" && !log.actionType.startsWith("BATCH")) return false;
                          if (auditActionFilter !== "BATCH" && log.actionType !== auditActionFilter) return false;
                        }
                        if (auditTextSearch.trim()) {
                          const q = auditTextSearch.toLowerCase();
                          return (log.userEmail?.toLowerCase().includes(q) || log.userName?.toLowerCase().includes(q) || log.details?.toLowerCase().includes(q) || log.targetName?.toLowerCase().includes(q) || log.targetId?.toLowerCase().includes(q) || log.userRole?.toLowerCase().includes(q));
                        }
                        return true;
                      }).length}
                    </strong>
                  </div>
                </div>

                <div className="bg-slate-50 border border-slate-200 p-4 rounded-xl flex items-center gap-4">
                  <div className="p-3 bg-red-100 text-red-800 rounded-lg">
                    <ShieldAlert className="w-5 h-5 text-red-600" />
                  </div>
                  <div>
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest block leading-none">Purges & Deletes</span>
                    <strong className="text-xl font-extrabold text-slate-900 leading-none block mt-1">
                      {auditLogs.filter(l => l.actionType && l.actionType.includes("DELETE")).length}
                    </strong>
                  </div>
                </div>

                <div className="bg-slate-50 border border-slate-200 p-4 rounded-xl flex items-center gap-4">
                  <div className="p-3 bg-blue-100 text-blue-800 rounded-lg">
                    <User className="w-5 h-5 text-blue-600" />
                  </div>
                  <div>
                    <span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest block leading-none">Active Operators</span>
                    <strong className="text-xl font-extrabold text-slate-900 leading-none block mt-1">
                      {new Set(auditLogs.map(l => l.userEmail)).size}
                    </strong>
                  </div>
                </div>
              </div>

              {/* LEDGER INSIGHTS & ANALYTICAL SUMMARY REPORT */}
              <div className="bg-slate-50 border border-slate-200 rounded-xl p-5 shadow-sm space-y-4">
                <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-2 border-b border-slate-100 pb-3">
                  <div className="flex items-center gap-2">
                    <History className="w-4 h-4 text-amber-500" />
                    <div>
                      <h3 className="text-sm font-bold text-slate-800 tracking-tight">Ledger Velocity & Security Analytics Statement</h3>
                      <p className="text-[10px] text-slate-400">Automated metrics highlighting operational activity, system peaks, and modified corporate elements.</p>
                    </div>
                  </div>
                  <div className="flex bg-slate-200 p-0.5 rounded-lg text-xs font-bold gap-1 self-stretch sm:self-auto shrink-0 select-none">
                    <button
                      onClick={() => setAuditInsightRange("30")}
                      className={`px-3 py-1 rounded transition-all cursor-pointer ${
                        auditInsightRange === "30" ? "bg-white text-slate-900 shadow-sm" : "text-slate-500 hover:text-slate-900"
                      }`}
                    >
                      Past 30 Days (Active)
                    </button>
                    <button
                      onClick={() => setAuditInsightRange("all")}
                      className={`px-3 py-1 rounded transition-all cursor-pointer ${
                        auditInsightRange === "all" ? "bg-white text-slate-900 shadow-sm" : "text-slate-500 hover:text-slate-900"
                      }`}
                    >
                      All-Time Complete
                    </button>
                  </div>
                </div>

                <div className="grid grid-cols-1 lg:grid-cols-3 gap-5">
                  {/* Left Column: Most Active Users */}
                  <div className="space-y-3">
                    <div className="flex items-center gap-1.5 border-b border-slate-100 pb-1.5">
                      <User className="w-3.5 h-3.5 text-blue-500" />
                      <span className="text-[10px] font-extrabold uppercase tracking-wider text-slate-500">Most Active Operators</span>
                    </div>
                    {auditActiveOperators.length > 0 ? (
                      <div className="space-y-2">
                        {auditActiveOperators.map((operator) => {
                          const maxCount = auditActiveOperators[0]?.count || 1;
                          const barWidth = Math.max(10, Math.round((operator.count / maxCount) * 100));
                          return (
                            <div key={operator.email} className="bg-white border border-slate-200/60 rounded-xl p-3 hover:border-slate-300 transition-all flex flex-col gap-2 shadow-none">
                              <div className="flex items-center justify-between gap-3">
                                <div className="flex items-center gap-2.5 overflow-hidden">
                                  <div className="w-8 h-8 rounded-full bg-blue-50 text-blue-600 flex items-center justify-center font-bold text-xs shrink-0 select-none">
                                    {operator.name.split(" ").map(n => n[0]).join("").toUpperCase() || "OP"}
                                  </div>
                                  <div className="overflow-hidden leading-tight">
                                    <div className="font-bold text-[11px] text-slate-900 truncate">{operator.name}</div>
                                    <div className="text-[9px] text-slate-400 font-mono truncate">{operator.email}</div>
                                  </div>
                                </div>
                                <div className="text-right shrink-0">
                                  <span className="text-xs font-extrabold text-slate-800">{operator.count} edits</span>
                                  <span className="text-[8px] text-slate-400 block font-bold uppercase">{operator.role}</span>
                                </div>
                              </div>
                              <div className="w-full bg-slate-100 h-1.5 rounded-full overflow-hidden">
                                <div className="bg-blue-500 h-full rounded-full transition-all duration-500" style={{ width: `${barWidth}%` }}></div>
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    ) : (
                      <div className="bg-white border border-dashed border-slate-200 rounded-xl text-center py-8 text-xs text-slate-400">
                        No active users logged in selection range.
                      </div>
                    )}
                  </div>

                  {/* Middle Column: Most Modified Entities */}
                  <div className="space-y-3">
                    <div className="flex items-center gap-1.5 border-b border-slate-100 pb-1.5">
                      <Shield className="w-3.5 h-3.5 text-purple-500" />
                      <span className="text-[10px] font-extrabold uppercase tracking-wider text-slate-500">Most Modified Elements</span>
                    </div>
                    {auditModifiedEntities.length > 0 ? (
                      <div className="space-y-2 font-sans">
                        {auditModifiedEntities.map((entity) => {
                          let targetColorClass = "bg-slate-100 text-slate-605";
                          if (entity.type === "Entity") targetColorClass = "bg-purple-50 text-purple-600 border border-purple-100";
                          else if (entity.type === "Contact") targetColorClass = "bg-green-50 text-green-600 border border-green-100";
                          else if (entity.type === "Interaction") targetColorClass = "bg-amber-50 text-amber-600 border border-amber-100";
                          else if (entity.type === "Engagement") targetColorClass = "bg-sky-50 text-sky-600 border border-sky-100";
                          else if (entity.type === "Document") targetColorClass = "bg-violet-50 text-violet-600 border border-violet-100";
                          else if (entity.type === "User") targetColorClass = "bg-pink-50 text-pink-600 border border-pink-100";

                          return (
                            <div key={entity.key} className="bg-white border border-slate-200/60 rounded-xl p-3 hover:border-slate-300 transition-all flex justify-between items-center gap-3">
                              <div className="overflow-hidden leading-tight">
                                <div className="font-bold text-[11px] text-slate-800 truncate" title={entity.name}>{entity.name}</div>
                                <div className="mt-1 flex items-center gap-1">
                                  <span className={`text-[8px] font-bold px-1 rounded ${targetColorClass}`}>
                                    {entity.type}
                                  </span>
                                  <span className="text-[8px] font-mono text-slate-400">#{entity.id.substring(0, 6)}</span>
                                </div>
                              </div>
                              <div className="text-right shrink-0">
                                <span className="text-xs font-extrabold text-slate-800">{entity.count} edits</span>
                                <div className="text-[8px] text-slate-400 flex gap-1 justify-end font-mono">
                                  {entity.creates > 0 && <span className="text-emerald-500 font-bold">C:{entity.creates}</span>}
                                  {entity.updates > 0 && <span className="text-blue-500 font-bold">U:{entity.updates}</span>}
                                  {entity.deletes > 0 && <span className="text-rose-500 font-bold">D:{entity.deletes}</span>}
                                </div>
                              </div>
                            </div>
                          );
                        })}
                      </div>
                    ) : (
                      <div className="bg-white border border-dashed border-slate-200 rounded-xl text-center py-8 text-xs text-slate-400">
                        No modified records logged in selection range.
                      </div>
                    )}
                  </div>

                  {/* Right Column: Security Indicators & Briefing */}
                  <div className="space-y-3">
                    <div className="flex items-center gap-1.5 border-b border-slate-100 pb-1.5">
                      <ShieldAlert className="w-3.5 h-3.5 text-amber-500" />
                      <span className="text-[10px] font-extrabold uppercase tracking-wider text-slate-500">Security intelligence briefing</span>
                    </div>
                    <div className="bg-white border border-slate-200 rounded-xl p-4 gap-3.5 flex flex-col justify-between h-[calc(100%-25px)] min-h-[170px]">
                      <div className="space-y-2 text-[11px] font-medium text-slate-600">
                        <div className="flex justify-between items-center bg-slate-50 p-2 rounded-lg border border-slate-100">
                          <span className="text-slate-400">Peak Security Activity Hr:</span>
                          <strong className="text-slate-800 font-mono text-[10px] bg-white border border-slate-200 px-1.5 py-0.5 rounded shadow-sm">{auditSystemMetrics.peakHour}</strong>
                        </div>
                        <div className="flex justify-between items-center bg-slate-50 p-2 rounded-lg border border-slate-100">
                          <span className="text-slate-400">System Delete Ratio:</span>
                          <strong className="text-rose-600 font-mono text-[10px] bg-white border border-slate-200 px-1.5 py-0.5 rounded shadow-sm">{auditSystemMetrics.deletePercent}</strong>
                        </div>
                        <div className="flex justify-between items-center bg-slate-50 p-2 rounded-lg border border-slate-100">
                          <span className="text-slate-400">Senior Analyst Actions Role:</span>
                          <strong className="text-blue-600 font-mono text-[10px] bg-white border border-slate-200 px-1.5 py-0.5 rounded shadow-sm">{auditSystemMetrics.adminActionPercent}</strong>
                        </div>
                      </div>
                      <div className="bg-amber-50/50 border border-amber-100/70 p-2 rounded-lg text-[9.5px] text-amber-800 font-semibold leading-relaxed">
                        ⚠️ <span className="font-bold underline">SECURITY SYSTEM INSIGHTS</span>: All logs listed above are pulled securely from SQLite persistence server to trace actions and verify operator session compliance.
                      </div>
                    </div>
                  </div>
                </div>
              </div>

              {/* Filtering Controls Row */}
              <div className="bg-white border border-slate-200 rounded-xl p-4 shadow-sm flex flex-col md:flex-row items-center gap-3.5">
                <div className="w-full md:w-1/3 relative">
                  <Search className="absolute left-3 top-2.5 w-4 h-4 text-slate-400" />
                  <input
                    type="text"
                    placeholder="Search by operator, detail message, target ID..."
                    value={auditTextSearch}
                    onChange={(e) => setAuditTextSearch(e.target.value)}
                    className="w-full pl-9 pr-4 py-1.5 border border-slate-200 rounded-lg text-xs font-semibold focus:outline-none focus:border-slate-400 font-mono"
                  />
                </div>

                <div className="w-full md:w-1/4 flex items-center gap-2">
                  <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider shrink-0">Target:</span>
                  <select
                    value={auditTargetFilter}
                    onChange={(e) => setAuditTargetFilter(e.target.value)}
                    className="w-full border border-slate-200 px-2.5 py-1.5 rounded-lg text-xs font-bold text-slate-700 bg-slate-50 cursor-pointer"
                  >
                    <option value="ALL">All Vault Classes</option>
                    <option value="Entity">Corporate Entities</option>
                    <option value="Contact">Contacts</option>
                    <option value="Interaction">Interactions</option>
                    <option value="Engagement">Engagements</option>
                    <option value="Note">Notes Ledger</option>
                    <option value="Document">Documents Vault</option>
                    <option value="User">Operator Seats</option>
                    <option value="Tag">Categorization Tags</option>
                  </select>
                </div>

                <div className="w-full md:w-1/4 flex items-center gap-2">
                  <span className="text-[10px] font-bold text-slate-400 uppercase tracking-wider shrink-0">Action:</span>
                  <select
                    value={auditActionFilter}
                    onChange={(e) => setAuditActionFilter(e.target.value)}
                    className="w-full border border-slate-200 px-2.5 py-1.5 rounded-lg text-xs font-bold text-slate-700 bg-slate-50 cursor-pointer"
                  >
                    <option value="ALL">All Operations</option>
                    <option value="CREATE">CREATE</option>
                    <option value="UPDATE">UPDATE</option>
                    <option value="DELETE">DELETE</option>
                    <option value="BATCH">BULK / BATCH ACTIONS</option>
                  </select>
                </div>

                <button
                  type="button"
                  onClick={handleExportAuditLogsToCSV}
                  className="px-3.5 py-1.5 bg-indigo-600 hover:bg-indigo-700 text-white rounded-lg text-xs font-bold transition flex items-center gap-1.5 shrink-0 cursor-pointer shadow-sm md:ml-auto"
                  title="Export currently filtered audit logs to a CSV spreadsheet"
                >
                  <Download className="w-3.5 h-3.5" />
                  <span>Export to CSV</span>
                </button>

                {(auditTextSearch || auditTargetFilter !== "ALL" || auditActionFilter !== "ALL") && (
                  <button
                    onClick={() => {
                      setAuditTextSearch("");
                      setAuditTargetFilter("ALL");
                      setAuditActionFilter("ALL");
                    }}
                    className="text-xs font-bold text-red-600 hover:text-red-700 hover:underline shrink-0 cursor-pointer"
                  >
                    Reset Filters
                  </button>
                )}
              </div>

              {/* Data Table / Timeline Ledger */}
              <div className="bg-white border border-slate-200 rounded-xl overflow-hidden shadow-sm">
                <div className="overflow-x-auto overflow-y-auto max-h-[600px]">
                  <table className="w-full text-left" id="audit-logs-table">
                    <thead>
                      <tr className="bg-slate-50 border-b border-slate-200 text-slate-500 font-bold text-[10px] uppercase tracking-wider sticky top-0 z-10">
                        <th className="p-4 py-3 bg-slate-50">Timestamp (UTC)</th>
                        <th className="p-4 py-3 bg-slate-50">Operator</th>
                        <th className="p-4 py-3 bg-slate-50">Action Type</th>
                        <th className="p-4 py-3 bg-slate-50">Target Subsystem</th>
                        <th className="p-4 py-3 bg-slate-50">Identified Target</th>
                        <th className="p-4 py-3 bg-slate-50">Transaction details</th>
                      </tr>
                    </thead>
                    <tbody className="divide-y divide-slate-100 text-xs text-slate-700">
                      {filteredAuditLogs.length > 0 ? (
                        filteredAuditLogs.map((log) => {
                          let actionColorClass = "bg-blue-50 text-blue-800 border-blue-200";
                          if (log.actionType === "CREATE") {
                            actionColorClass = "bg-emerald-50 text-emerald-800 border-emerald-200";
                          } else if (log.actionType && log.actionType.includes("DELETE")) {
                            actionColorClass = "bg-rose-50 text-rose-800 border-rose-200";
                          } else if (log.actionType && log.actionType.includes("BATCH")) {
                            actionColorClass = "bg-purple-50 text-purple-800 border-purple-200";
                          }

                          let targetColorClass = "bg-slate-100 text-slate-800";
                          if (log.targetType === "Entity") targetColorClass = "bg-purple-100 text-purple-800";
                          else if (log.targetType === "Contact") targetColorClass = "bg-green-100 text-green-800";
                          else if (log.targetType === "Interaction") targetColorClass = "bg-amber-100 text-amber-800";
                          else if (log.targetType === "Engagement") targetColorClass = "bg-sky-100 text-sky-800";
                          else if (log.targetType === "Document") targetColorClass = "bg-violet-100 text-violet-800";
                          else if (log.targetType === "User") targetColorClass = "bg-pink-100 text-pink-800";

                          return (
                            <tr
                              key={log.id}
                              className="hover:bg-slate-50/50 transition cursor-default font-semibold animate-in fade-in duration-100"
                            >
                              {/* Timestamp */}
                              <td className="p-4 font-mono text-[11px] text-slate-500 shrink-0 whitespace-nowrap">
                                {new Date(log.timestamp).toLocaleString("sv-SE", { timeZone: "UTC" }).split(" ")[0]}&nbsp;
                                <span className="text-[10px] text-slate-400 font-normal">
                                  {new Date(log.timestamp).toLocaleTimeString("en-GB", { hour: "2-digit", minute: "2-digit", second: "2-digit", timeZone: "UTC" })}
                                </span>
                              </td>

                              {/* Operator details */}
                              <td className="p-4 leading-normal">
                                <div className="font-extrabold text-slate-900 leading-tight block">{log.userName}</div>
                                <span className="text-[10px] text-slate-400 font-mono">{log.userEmail}</span>
                                <span className="inline-block text-[8px] font-extrabold text-slate-400 bg-slate-50 border border-slate-200 px-1 py-0.2 ml-1.5 uppercase rounded">
                                  {log.userRole}
                                </span>
                              </td>

                              {/* Action Type badge */}
                              <td className="p-4">
                                <span className={`inline-flex items-center text-[9px] font-extrabold px-2 py-0.5 rounded-full border ${actionColorClass}`}>
                                  {log.actionType}
                                </span>
                              </td>

                              {/* Target Subsystem badge */}
                              <td className="p-4">
                                <span className={`inline-flex items-center text-[9px] font-bold px-2 py-0.5 rounded ${targetColorClass}`}>
                                  {log.targetType}
                                </span>
                              </td>

                              {/* Identified Target */}
                              <td className="p-4 max-w-[150px] truncate leading-tight">
                                <span className="font-mono text-[10px] text-slate-400 block mb-0.5">#{log.targetId ? log.targetId.substr(0, 8) : "N/A"}</span>
                                <span className="font-bold text-slate-800" title={log.targetName}>{log.targetName || "N/A"}</span>
                              </td>

                              {/* Transaction Details message */}
                              <td className="p-4 leading-relaxed max-w-[280px]">
                                <p className="text-slate-600 font-medium font-mono text-[11px] break-words">{log.details}</p>
                              </td>
                            </tr>
                          );
                        })
                      ) : (
                        <tr>
                          <td colSpan={6} className="p-8 text-center text-slate-400">
                            No logs found matching selected constraints. Add some interactions, update entities, or change contacts to generate logs!
                          </td>
                        </tr>
                      )}
                    </tbody>
                  </table>
                </div>
              </div>
            </div>
          )}
        </div>
      </main>

      {/* DYNAMIC GLOBAL SEARCH RESULTS MODAL */}
      {isSearchActive && (
        <div id="search-results-modal" className="fixed inset-0 bg-slate-900/40 backdrop-blur-[2px] flex items-center justify-center z-50 p-12">
          <div className="fixed inset-0" onClick={() => { setIsSearchActive(false); setSearchQuery(""); }} />

          <div className="bg-white w-[600px] max-h-[500px] rounded-2xl shadow-2xl flex flex-col overflow-hidden border border-slate-200 z-[60] animate-in zoom-in-95 duration-200">
            <div className="p-4 border-b border-slate-100 bg-slate-50/50 flex justify-between items-center">
              <span className="flex items-center gap-2 text-slate-500 text-xs font-bold uppercase">
                <Search className="w-4 h-4" /> Global Database Results for &quot;{searchQuery}&quot;
              </span>
              <button onClick={() => { setIsSearchActive(false); setSearchQuery(""); }} className="text-slate-400 hover:text-slate-600">
                <X className="w-5 h-5" />
              </button>
            </div>

            <div className="flex-1 overflow-y-auto p-4 space-y-6">
              <section>
                <h4 className="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-2">Interactions ({searchResults.interactions.length})</h4>
                {searchResults.interactions.length === 0 ? <p className="text-xs text-slate-400 italic">No matches</p> : (
                  <div className="space-y-1">
                    {searchResults.interactions.map((i) => (
                      <div key={i.id} onClick={() => { setSelectedItem({ dataType: "interaction", data: i }); setIsSearchActive(false); setSearchQuery(""); }} className="p-2 hover:bg-slate-50 border border-transparent hover:border-slate-100 rounded-lg cursor-pointer">
                        <p className="font-semibold text-xs text-slate-900">{i.subject}</p>
                        <p className="text-[10px] text-slate-500">Corporate client mapping: {i.client}</p>
                      </div>
                    ))}
                  </div>
                )}
              </section>

              <section>
                <h4 className="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-2">Contacts ({searchResults.contacts.length})</h4>
                {searchResults.contacts.length === 0 ? <p className="text-xs text-slate-400 italic">No matches</p> : (
                  <div className="space-y-1">
                    {searchResults.contacts.map((c) => (
                      <div key={c.id} onClick={() => { setSelectedItem({ dataType: "contact", data: c }); setIsSearchActive(false); setSearchQuery(""); }} className="p-2 hover:bg-slate-50 border border-transparent hover:border-slate-100 rounded-lg cursor-pointer">
                        <p className="font-semibold text-xs text-slate-900">{c.name}</p>
                        <p className="text-[10px] text-slate-500">{c.role} at {c.company}</p>
                      </div>
                    ))}
                  </div>
                )}
              </section>

              <section>
                <h4 className="text-[10px] font-bold text-slate-400 uppercase tracking-widest mb-2">Corporate Entities ({searchResults.entities.length})</h4>
                {searchResults.entities.length === 0 ? <p className="text-xs text-slate-400 italic">No matches</p> : (
                  <div className="space-y-1">
                    {searchResults.entities.map((e) => (
                      <div key={e.id} onClick={() => { setSelectedItem({ dataType: "entity", data: e }); setIsSearchActive(false); setSearchQuery(""); }} className="p-2 hover:bg-slate-50 border border-transparent hover:border-slate-100 rounded-lg cursor-pointer">
                        <p className="font-semibold text-xs text-slate-900">{e.name}</p>
                        <p className="text-[10px] text-slate-500">{e.industry} and based in {e.location}</p>
                      </div>
                    ))}
                  </div>
                )}
              </section>
            </div>
          </div>
        </div>
      )}

      {/* CREATE NEW WORKSPACE ENTRY MODAL */}
      {isNewModalOpen && (
        <div className="fixed inset-0 bg-slate-900/40 backdrop-blur-[2px] flex items-center justify-center z-[80] p-4">
          <div className="fixed inset-0" onClick={() => setIsNewModalOpen(false)} />

          <div className="bg-white w-full max-w-lg rounded-xl shadow-2xl flex flex-col overflow-hidden border border-slate-200 z-[90] animate-in zoom-in-95 duration-200 text-xs">
            <div className="bg-slate-50 p-4 border-b border-slate-100 flex justify-between items-center font-bold text-slate-800">
              <span>Register New Workspace Coordinates</span>
              <button onClick={() => setIsNewModalOpen(false)} className="text-slate-400 hover:text-slate-600"><X className="w-5 h-5" /></button>
            </div>

             <div className="px-4 py-2 bg-slate-50/50 border-b border-slate-100 flex flex-wrap gap-1 justify-center font-bold uppercase tracking-wider text-[10px] text-slate-500">
              <button type="button" onClick={() => setNewType("interaction")} className={`px-2.5 py-1 rounded transition ${newType === "interaction" ? "bg-slate-950 text-white" : "hover:bg-slate-200"}`}>Interaction</button>
              <button type="button" onClick={() => setNewType("contact")} className={`px-2.5 py-1 rounded transition ${newType === "contact" ? "bg-slate-950 text-white" : "hover:bg-slate-200"}`}>Contact</button>
              <button type="button" onClick={() => setNewType("entity")} className={`px-2.5 py-1 rounded transition ${newType === "entity" ? "bg-slate-950 text-white" : "hover:bg-slate-200"}`}>Corporate</button>
              <button type="button" onClick={() => setNewType("engagement")} className={`px-2.5 py-1 rounded transition ${newType === "engagement" ? "bg-slate-950 text-white" : "hover:bg-slate-200"}`}>Engagement</button>
              <button type="button" onClick={() => setNewType("user")} className={`px-2.5 py-1 rounded transition ${newType === "user" ? "bg-slate-950 text-white" : "hover:bg-slate-200"}`}>Operator</button>
            </div>

            <form onSubmit={handleCreateEntry} className="p-6 space-y-4 max-h-[450px] overflow-y-auto">
              {newType === "interaction" && (
                <div className="space-y-3">
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Subject Briefing</label>
                    <input type="text" required value={intForm.subject} onChange={(e) => setIntForm({ ...intForm, subject: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg focus:bg-white outline-none animate-in fade-in" />
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Type</label>
                      <select value={intForm.type} onChange={(e) => setIntForm({ ...intForm, type: e.target.value as any })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none font-semibold text-slate-705">
                        <option value="Meeting">Meeting</option>
                        <option value="Email Sync">Email Sync</option>
                        <option value="Support Call">Support Call</option>
                        <option value="Review Session">Review Session</option>
                      </select>
                    </div>
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Target Date</label>
                      <input type="date" required value={intForm.date} onChange={(e) => setIntForm({ ...intForm, date: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none animate-in fade-in" />
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Assignee</label>
                      <select value={intForm.assignee} onChange={(e) => setIntForm({ ...intForm, assignee: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none text-slate-705">
                        {ASSIGNEES.map((a) => <option key={a} value={a}>{a}</option>)}
                      </select>
                    </div>
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Corporate Client</label>
                      <select value={intForm.client} onChange={(e) => setIntForm({ ...intForm, client: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none text-slate-705">
                        <option value="">-- Choose Account --</option>
                        {entities.map((e) => <option key={e.id} value={e.name}>{e.name}</option>)}
                      </select>
                    </div>
                  </div>
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Interaction Summary Brief</label>
                    <textarea rows={3} value={intForm.summary} onChange={(e) => setIntForm({ ...intForm, summary: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none animate-in fade-in" placeholder="Milestone deliverables details..." />
                  </div>
                </div>
              )}

              {newType === "contact" && (
                <div className="space-y-3">
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Stakeholder Full Name</label>
                    <input type="text" required value={contactForm.name} onChange={(e) => setContactForm({ ...contactForm, name: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none animate-in" />
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Corporate Role</label>
                      <input type="text" required value={contactForm.role} onChange={(e) => setContactForm({ ...contactForm, role: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="VP of Operations" />
                    </div>
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Associated Company</label>
                      <select value={contactForm.company} onChange={(e) => setContactForm({ ...contactForm, company: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none text-slate-705">
                        <option value="">-- Choose Corporate --</option>
                        {entities.map((e) => <option key={e.id} value={e.name}>{e.name}</option>)}
                      </select>
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Coordinates Email</label>
                      <input type="email" required value={contactForm.email} onChange={(e) => setContactForm({ ...contactForm, email: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" />
                    </div>
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Primary Desk Phone</label>
                      <input type="tel" value={contactForm.phone} onChange={(e) => setContactForm({ ...contactForm, phone: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="+1 (555) 444-2211" />
                    </div>
                  </div>
                </div>
              )}

              {newType === "entity" && (
                <div className="space-y-3">
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Corporate Organization Name</label>
                    <input type="text" required value={entityForm.name} onChange={(e) => setEntityForm({ ...entityForm, name: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" />
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Industry Classification</label>
                      <input type="text" required value={entityForm.industry} onChange={(e) => setEntityForm({ ...entityForm, industry: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="SaaS Software" />
                    </div>
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Account Relationship Tier</label>
                      <select value={entityForm.tier} onChange={(e) => setEntityForm({ ...entityForm, tier: e.target.value as any })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none text-slate-705">
                        <option value="Strategic">Strategic</option>
                        <option value="Enterprise">Enterprise</option>
                        <option value="Key Account">Key Account</option>
                        <option value="Growth">Growth</option>
                      </select>
                    </div>
                  </div>
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Headquarters Geolocation</label>
                    <input type="text" required value={entityForm.location} onChange={(e) => setEntityForm({ ...entityForm, location: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="London, UK" />
                  </div>
                </div>
              )}

              {newType === "engagement" && (
                <div className="space-y-3">
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Engagement Title</label>
                    <input type="text" required value={engagementForm.title} onChange={(e) => setEngagementForm({ ...engagementForm, title: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="Strategic Expansion SOW" />
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Associated Corporate Client</label>
                      <select value={engagementForm.client} onChange={(e) => setEngagementForm({ ...engagementForm, client: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none text-slate-705">
                        <option value="">-- Choose Corporate --</option>
                        {entities.map((e) => <option key={e.id} value={e.name}>{e.name}</option>)}
                      </select>
                    </div>
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Valuation SOW Budget</label>
                      <input type="text" required value={engagementForm.budget} onChange={(e) => setEngagementForm({ ...engagementForm, budget: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="$75,000" />
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Engagement Type</label>
                      <input type="text" required value={engagementForm.type} onChange={(e) => setEngagementForm({ ...engagementForm, type: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="Consulting Retainer" />
                    </div>
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Executive Status</label>
                      <select value={engagementForm.status} onChange={(e) => setEngagementForm({ ...engagementForm, status: e.target.value as any })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none">
                        <option value="Active">Active Line</option>
                        <option value="Under Negotiation">Under Negotiation</option>
                        <option value="Pending Draft">Pending Draft</option>
                        <option value="Closed">Closed</option>
                      </select>
                    </div>
                  </div>
                  <div className="grid grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Initiative Commencement</label>
                      <input type="date" required value={engagementForm.startDate} onChange={(e) => setEngagementForm({ ...engagementForm, startDate: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" />
                    </div>
                    <div>
                      <label className="block text-slate-505 font-bold mb-1">Term Completion</label>
                      <input type="date" required value={engagementForm.endDate} onChange={(e) => setEngagementForm({ ...engagementForm, endDate: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" />
                    </div>
                  </div>
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Engagement Objective Briefing</label>
                    <textarea rows={3} value={engagementForm.description} onChange={(e) => setEngagementForm({ ...engagementForm, description: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="Scope statement and deliverables checklist..." />
                  </div>
                </div>
              )}

              {newType === "user" && (
                <div className="space-y-3">
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Operator Profile Full Name</label>
                    <input type="text" required value={operatorForm.name} onChange={(e) => setOperatorForm({ ...operatorForm, name: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="Operator Name" />
                  </div>
                  <div className="grid grid-cols-1 md:grid-cols-2 gap-3">
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Corporate Seat Email</label>
                      <input type="email" required value={operatorForm.email} onChange={(e) => setOperatorForm({ ...operatorForm, email: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="username@enterprise.com" />
                    </div>
                    <div>
                      <label className="block text-slate-500 font-bold mb-1">Operator System Role</label>
                      <select value={operatorForm.role} onChange={(e) => setOperatorForm({ ...operatorForm, role: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none text-slate-705 font-semibold">
                        <option value="Senior Analyst">Senior Analyst</option>
                        <option value="Senior Operator">Senior Operator</option>
                        <option value="Auditor Seat">Auditor Seat</option>
                        <option value="Associate Coordinator">Associate Coordinator</option>
                        <option value="System Security Agent">System Security Agent</option>
                      </select>
                    </div>
                  </div>
                  <div>
                    <label className="block text-slate-500 font-bold mb-1">Access passphrase</label>
                    <input type="password" required value={operatorForm.passphrase} onChange={(e) => setOperatorForm({ ...operatorForm, passphrase: e.target.value })} className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none" placeholder="••••••••" />
                    <span className="block text-[10px] text-slate-400 mt-1">This seat passphrase will be securely hashed with SHA variants in our system sandbox.</span>
                  </div>
                </div>
              )}

              <div className="pt-4 border-t flex justify-end gap-2 text-xs">
                <button type="button" onClick={() => setIsNewModalOpen(false)} className="px-4 py-2 hover:bg-slate-100 rounded-lg text-slate-500 font-bold">Cancel</button>
                <button type="submit" className="px-4 py-2 bg-blue-600 hover:bg-blue-700 text-white rounded-lg font-bold">Register Sync</button>
              </div>
            </form>
          </div>
        </div>
      )}

      {/* RIGHT SLIDEOUT CONFIGURATION DRAWER */}
      {selectedItem && (
        <div className="fixed inset-0 bg-slate-900/40 backdrop-blur-[2px] flex justify-end z-[80] animate-in fade-in duration-200">
          <div className="fixed inset-0" onClick={() => setSelectedItem(null)} />

          <div className="bg-white w-[420px] h-full shadow-2xl border-l border-slate-200 z-[90] flex flex-col justify-between animate-in slide-in-from-right duration-200">

            <div className="overflow-y-auto flex-1 text-xs">

              {/* Header */}
              <div className="p-5 border-b border-slate-150 bg-slate-50/50 flex justify-between items-center text-slate-800">
                <div>
                  <span className="text-[10px] font-bold text-slate-400 uppercase tracking-widest block mb-0.5">Configuration Drawer</span>
                  <h3 className="font-extrabold text-sm tracking-tight leading-none uppercase">Database Link Console</h3>
                </div>
                <button onClick={() => setSelectedItem(null)} className="p-1 hover:bg-slate-200 rounded text-slate-400">
                  <X className="w-5 h-5" />
                </button>
              </div>

              {/* Editable Fields */}
              <div className="p-6 space-y-5">
                {selectedItem.dataType === "interaction" && (
                  <div className="space-y-4">
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Subject Briefing</label>
                      <input
                        type="text"
                        value={selectedItem.data.subject}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, subject: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg focus:bg-white outline-none font-bold text-slate-800"
                      />
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Operator Assignee</label>
                        <select
                          value={selectedItem.data.assignee}
                          onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, assignee: e.target.value } })}
                          className="w-full bg-slate-50 border p-2.5 rounded-lg font-semibold text-slate-700"
                        >
                          {ASSIGNEES.map((a) => <option key={a} value={a}>{a}</option>)}
                        </select>
                      </div>
                      <div>
                        <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Status Status</label>
                        <select
                          value={selectedItem.data.status}
                          onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, status: e.target.value as any } })}
                          className="w-full bg-slate-50 border p-2.5 rounded-lg font-semibold text-slate-700"
                        >
                          <option value="IN PROGRESS">IN PROGRESS</option>
                          <option value="SCHEDULED">SCHEDULED</option>
                          <option value="COMPLETED">COMPLETED</option>
                          <option value="BLOCKED">BLOCKED</option>
                        </select>
                      </div>
                    </div>
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Operational Summary Details</label>
                      <textarea
                        rows={4}
                        value={selectedItem.data.summary}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, summary: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none"
                      />
                    </div>
                  </div>
                )}

                {selectedItem.dataType === "contact" && (
                  <div className="space-y-4">
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Full Stakeholder Name</label>
                      <input
                        type="text"
                        value={selectedItem.data.name}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, name: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg font-bold outline-none text-slate-800"
                      />
                    </div>
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Corporate Role Title</label>
                      <input
                        type="text"
                        value={selectedItem.data.role}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, role: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg font-semibold outline-none text-slate-700"
                      />
                    </div>
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Primary Email Contact</label>
                      <input
                        type="email"
                        value={selectedItem.data.email}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, email: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg font-semibold outline-none"
                      />
                    </div>
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Desk Phone Number</label>
                      <input
                        type="tel"
                        value={selectedItem.data.phone}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, phone: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none font-mono"
                      />
                    </div>
                  </div>
                )}

                {selectedItem.dataType === "entity" && (
                  <div className="space-y-4">
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Corporate Entity Name</label>
                      <input
                        type="text"
                        value={selectedItem.data.name}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, name: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg font-bold outline-none text-slate-800"
                      />
                    </div>
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Sector Classification</label>
                      <input
                        type="text"
                        value={selectedItem.data.industry}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, industry: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg font-semibold outline-none"
                      />
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Relationship Tier</label>
                        <select
                          value={selectedItem.data.tier}
                          onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, tier: e.target.value as any } })}
                          className="w-full bg-slate-50 border p-2.5 rounded-lg font-semibold outline-none"
                        >
                          <option value="Strategic">Strategic</option>
                          <option value="Enterprise">Enterprise</option>
                          <option value="Key Account">Key Account</option>
                          <option value="Growth">Growth</option>
                        </select>
                      </div>
                      <div>
                        <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Headquarters Location</label>
                        <input
                          type="text"
                          value={selectedItem.data.location}
                          onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, location: e.target.value } })}
                          className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none"
                        />
                      </div>
                    </div>
                  </div>
                )}

                {selectedItem.dataType === "engagement" && (
                  <div className="space-y-4 animate-in fade-in">
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Engagement Title</label>
                      <input
                        type="text"
                        value={selectedItem.data.title}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, title: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg font-bold outline-none text-slate-800"
                        required
                      />
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Valuation Budget</label>
                        <input
                          type="text"
                          value={selectedItem.data.budget}
                          onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, budget: e.target.value } })}
                          className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none font-semibold text-slate-705"
                        />
                      </div>
                      <div>
                        <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Agreement Type</label>
                        <input
                          type="text"
                          value={selectedItem.data.type}
                          onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, type: e.target.value } })}
                          className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none font-semibold text-slate-705"
                        />
                      </div>
                    </div>
                    <div className="grid grid-cols-2 gap-3">
                      <div>
                        <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Commencement Term</label>
                        <input
                          type="date"
                          value={selectedItem.data.startDate}
                          onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, startDate: e.target.value } })}
                          className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none text-slate-700 font-mono"
                        />
                      </div>
                      <div>
                        <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Termination Term</label>
                        <input
                          type="date"
                          value={selectedItem.data.endDate}
                          onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, endDate: e.target.value } })}
                          className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none text-slate-700 font-mono"
                        />
                      </div>
                    </div>
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Executive Status</label>
                      <select
                        value={selectedItem.data.status}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, status: e.target.value as any } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg font-bold text-slate-700"
                      >
                        <option value="Active">Active Line</option>
                        <option value="Under Negotiation">Under Negotiation</option>
                        <option value="Pending Draft">Pending Draft</option>
                        <option value="Closed">Closed</option>
                      </select>
                    </div>
                    <div>
                      <label className="block text-[10px] font-bold text-slate-400 uppercase tracking-wide mb-1">Engagement Objective Briefing</label>
                      <textarea
                        rows={4}
                        value={selectedItem.data.description}
                        onChange={(e) => setSelectedItem({ ...selectedItem, data: { ...selectedItem.data, description: e.target.value } })}
                        className="w-full bg-slate-50 border p-2.5 rounded-lg outline-none leading-relaxed text-slate-700"
                      />
                    </div>
                  </div>
                )}

                {/* DYNAMIC ATTRIBUTES CONNECTOR / LINK MATRIX */}
                <div className="mt-8 pt-6 border-t border-slate-200">
                  <h4 className="font-bold text-slate-900 text-[11px] tracking-tight uppercase mb-4 flex items-center gap-1.5">
                    <Link className="w-4 h-4 text-blue-600" /> Linked Ecosystem Matrix
                  </h4>

                  {/* Tabs toggle */}
                  <div className="flex bg-slate-100 p-0.5 rounded-lg text-[10px] font-bold text-slate-500 uppercase mb-4">
                    <button
                      type="button"
                      onClick={() => setDrawerTab("tags")}
                      className={`flex-1 py-1 px-2 rounded-md transition ${drawerTab === "tags" ? "bg-white text-slate-900 shadow-sm" : "hover:text-slate-800"}`}
                    >
                      Tags ({linkedTags.length})
                    </button>
                    <button
                      type="button"
                      onClick={() => setDrawerTab("notes")}
                      className={`flex-1 py-1 px-2 rounded-md transition ${drawerTab === "notes" ? "bg-white text-slate-900 shadow-sm" : "hover:text-slate-800"}`}
                    >
                      Notes ({linkedNotes.length})
                    </button>
                    <button
                      type="button"
                      onClick={() => setDrawerTab("docs")}
                      className={`flex-1 py-1 px-2 rounded-md transition ${drawerTab === "docs" ? "bg-white text-slate-900 shadow-sm" : "hover:text-slate-800"}`}
                    >
                      Docs ({linkedDocs.length})
                    </button>
                  </div>

                  {/* TAB 1: TAG CONNECTIVITY */}
                  {drawerTab === "tags" && (
                    <div className="space-y-4">
                      {selectedItem.dataType !== "interaction" ? (
                        <p className="text-[11px] text-slate-400 italic">Global search matches tag associations within interactions and corresponding files indices.</p>
                      ) : (
                        <div className="space-y-3">
                          <div className="flex flex-wrap gap-1.5">
                            {linkedTags.length === 0 ? <span className="text-[11px] text-slate-400 italic">No assigned tags</span> : (
                              linkedTags.map((t) => (
                                <span key={t.id} className={`inline-flex items-center gap-1 px-2 py-0.5 rounded-md text-[10px] font-bold border ${getColorClasses(t.color)}`}>
                                  #{t.name}
                                  <button type="button" onClick={() => handleUnlinkTag(t.id)} className="text-[10px] text-slate-400 hover:text-slate-700 font-bold ml-1">×</button>
                                </span>
                              ))
                            )}
                          </div>

                          <div className="flex gap-1.5 pt-1">
                            <select
                              value={selectedTagToLink}
                              onChange={(e) => setSelectedTagToLink(e.target.value)}
                              className="flex-1 bg-slate-50 border p-2 rounded-lg text-xs"
                            >
                              <option value="">-- Assign Tag --</option>
                              {availableUnlinkedTags.map((t) => <option key={t.id} value={t.id}>#{t.name}</option>)}
                            </select>
                            <button type="button" onClick={handleLinkTag} className="px-3.5 bg-slate-900 hover:bg-slate-800 text-white rounded-lg font-bold">Link</button>
                          </div>

                          {/* Create dynamic tag in place */}
                          <div className="flex gap-1.5 pt-1 border-t border-slate-100 mt-2">
                            <input type="text" placeholder="New tag name..." value={newTagName} onChange={(e) => setNewTagName(e.target.value)} className="flex-1 bg-slate-50 border p-2 rounded-lg text-xs" />
                            <select value={newTagColor} onChange={(e) => setNewTagColor(e.target.value as any)} className="bg-slate-50 border p-2 rounded-lg text-xs font-semibold text-slate-700">
                              <option value="blue">Blue</option>
                              <option value="red">Red</option>
                              <option value="emerald">Green</option>
                              <option value="amber">Yellow</option>
                              <option value="purple">Purple</option>
                            </select>
                            <button type="button" onClick={handleCreateAndLinkTag} className="px-3 bg-blue-600 text-white rounded-lg font-bold">Add</button>
                          </div>
                        </div>
                      )}
                    </div>
                  )}

                  {/* TAB 2: RICH MEMO NOTES LOGGING */}
                  {drawerTab === "notes" && (
                    <div className="space-y-4">
                      <div className="space-y-2 max-h-48 overflow-y-auto pr-1">
                        {linkedNotes.length === 0 ? <p className="text-[11px] text-slate-400 italic">No notes linked</p> : (
                          linkedNotes.map((n) => (
                            <div
                              key={n.id}
                              className={`p-2.5 rounded-lg relative group leading-relaxed text-[11px] border transition ${
                                n.pinned
                                  ? "bg-indigo-50/40 border-indigo-200"
                                  : "bg-slate-50 border-slate-150"
                              }`}
                            >
                              <div className="absolute top-1 right-1 flex items-center gap-1">
                                <button
                                  type="button"
                                  onClick={() => handleTogglePinNote(n.id)}
                                  className={`p-0.5 rounded transition cursor-pointer ${
                                    n.pinned
                                      ? "text-indigo-600"
                                      : "opacity-0 group-hover:opacity-100 text-slate-400 hover:text-indigo-600"
                                  }`}
                                  title={n.pinned ? "Unpin Note" : "Pin Note"}
                                >
                                  {n.pinned ? (
                                    <Pin className="w-3.5 h-3.5 fill-indigo-600" />
                                  ) : (
                                    <Pin className="w-3.5 h-3.5" />
                                  )}
                                </button>
                                <button
                                  type="button"
                                  onClick={() => handleDeleteNote(n.id)}
                                  className="opacity-0 group-hover:opacity-100 text-slate-400 hover:text-rose-600 p-0.5 cursor-pointer"
                                >
                                  <Trash2 className="w-3.5 h-3.5" />
                                </button>
                              </div>
                              <p className="text-slate-800 pr-8">{n.content}</p>
                              <div className="mt-1 flex justify-between items-center text-[9px] text-slate-400 font-mono font-semibold">
                                <span>{n.author}</span>
                                <span>{new Date(n.createdAt).toLocaleDateString()}</span>
                              </div>
                            </div>
                          ))
                        )}
                      </div>

                      <div className="pt-2 border-t border-slate-100">
                        <textarea
                          placeholder="Type an operational workspace note..."
                          rows={2}
                          value={newNoteContent}
                          onChange={(e) => setNewNoteContent(e.target.value)}
                          className="w-full bg-slate-50 border p-2 rounded-lg text-xs outline-none focus:bg-white resize-none"
                        />
                        <div className="flex justify-between items-center mt-1">
                          <span className="text-[9px] text-slate-400 font-mono">Logger: {session.name}</span>
                          <button type="button" onClick={handleAddNote} className="px-3 py-1 bg-blue-600 text-white rounded-md font-bold text-[11px]">Save Note</button>
                        </div>
                      </div>
                    </div>
                  )}

                  {/* TAB 3: DOCUMENT UPLOAD AND ATTACHMENTS */}
                  {drawerTab === "docs" && (
                    <div className="space-y-4">
                      <div className="space-y-1.5 max-h-48 overflow-y-auto">
                        {linkedDocs.length === 0 ? <p className="text-[11px] text-slate-400 italic">No attached documents.</p> : (
                          linkedDocs.map((d) => (
                            <div key={d.id} className="flex items-center justify-between p-2 bg-slate-50 border border-slate-150 rounded-lg text-[11.5px]">
                              <div className="flex items-center gap-1.5 overflow-hidden">
                                <Paperclip className="w-3.5 h-3.5 text-slate-400 shrink-0" />
                                <span className="font-semibold text-slate-800 truncate" title={d.title}>{d.title}</span>
                                <span className="text-[9px] font-mono text-slate-400">({d.fileType})</span>
                              </div>
                              <div className="flex items-center gap-1.5 ml-2 shrink-0">
                                <span className="text-[9.5px] text-slate-400 font-mono">{d.fileSize}</span>
                                <button type="button" onClick={() => handleDeleteDocument(d.id)} className="text-slate-400 hover:text-rose-600">
                                  <X className="w-3.5 h-3.5" />
                                </button>
                              </div>
                            </div>
                          ))
                        )}
                      </div>

                      <div className="pt-2 border-t border-slate-100 space-y-2">
                        <div className="flex gap-1.5">
                          <input type="text" placeholder="filename (e.g. alignment_NDA)" value={newDocTitle} onChange={(e) => setNewDocTitle(e.target.value)} className="flex-1 bg-slate-50 border p-2 rounded-lg text-xs" />
                          <select
                            value={newDocType}
                            onChange={(e) => setNewDocType(e.target.value as any)}
                            className="bg-slate-50 border p-2 rounded-lg text-xs font-semibold text-slate-700"
                          >
                            <option value="PDF">PDF</option>
                            <option value="Spreadsheet">XLSX</option>
                            <option value="Contract">Contract</option>
                            <option value="Presentation">PPTX</option>
                            <option value="Briefing">Doc</option>
                          </select>
                        </div>
                        <div className="flex gap-1.5">
                          <input type="text" placeholder="Size (e.g. 1.2 MB)" value={newDocSize} onChange={(e) => setNewDocSize(e.target.value)} className="w-[100px] bg-slate-50 border p-2 rounded-lg text-xs" />
                          <button type="button" onClick={handleAddDocument} className="flex-1 bg-blue-600 text-white rounded-lg font-bold font-sans">Attach File</button>
                        </div>
                      </div>
                    </div>
                  )}

                </div>

              </div>
            </div>

            {/* Footer with discard permanence details and final updates */}
            <div className="p-4 border-t border-slate-150 bg-slate-50 flex items-center justify-between text-xs">
              <button
                type="button"
                onClick={() => handleDeleteItem(selectedItem.dataType, selectedItem.data.id)}
                className="px-3 py-2 bg-red-50 hover:bg-red-100 border border-red-200 text-red-600 rounded-lg font-bold flex items-center gap-1 transition"
              >
                <Trash2 className="w-3.5 h-3.5" /> Permanent Discard
              </button>

              <div className="flex gap-2">
                <button type="button" onClick={() => setSelectedItem(null)} className="px-3 py-2 hover:bg-slate-200 rounded-lg text-slate-500 font-bold">Cancel</button>
                <button type="button" onClick={() => handleUpdateItem(selectedItem.dataType, selectedItem.data)} className="px-4 py-2 bg-slate-900 text-white rounded-lg font-bold shadow">Save Changes</button>
              </div>
            </div>

          </div>
        </div>
      )}

      {/* FLOATING BATCH ACTIONS TOOLBAR */}
      <AnimatePresence>
        {((activeTab === "interactions" && selectedInteractionIds.length > 0) ||
          (activeTab === "contacts" && selectedContactIds.length > 0)) && (
          <motion.div
            initial={{ opacity: 0, y: 100, scale: 0.95 }}
            animate={{ opacity: 1, y: 0, scale: 1 }}
            exit={{ opacity: 0, y: 100, scale: 0.95 }}
            transition={{ type: "spring", stiffness: 150, damping: 20 }}
            className="fixed bottom-6 left-1/2 -translate-x-1/2 z-[80] w-[95%] max-w-4xl p-4 bg-slate-950 text-slate-100 shadow-[0_20px_50px_rgba(0,0,0,0.5)] border border-slate-850 rounded-2xl flex flex-col md:flex-row items-center justify-between gap-4"
          >
            <div className="flex items-center gap-3">
              <div className="w-10 h-10 rounded-xl bg-blue-600/20 text-blue-400 flex items-center justify-center font-bold font-mono">
                {activeTab === "interactions" ? selectedInteractionIds.length : selectedContactIds.length}
              </div>
              <div>
                <h4 className="text-sm font-extrabold text-white tracking-tight">
                  {activeTab === "interactions" ? "Batch Interaction Operations" : "Batch Contact Operations"}
                </h4>
                <p className="text-[10px] text-slate-400 font-semibold leading-relaxed">
                  {activeTab === "interactions"
                    ? `Specify actions to apply for ${selectedInteractionIds.length} selected sequence items`
                    : `Specify role/company adjustments or deletion for ${selectedContactIds.length} selected profiles`}
                </p>
              </div>
            </div>

            {/* Interactions Batch Panel Actions */}
            {activeTab === "interactions" && (
              <div className="flex flex-wrap items-center gap-2">
                <span className="text-[10px] font-bold text-slate-500 uppercase tracking-wider font-mono mr-1">Update Status:</span>
                {(["IN PROGRESS", "SCHEDULED", "COMPLETED", "BLOCKED"] as any[]).map((st) => (
                  <button
                    key={st}
                    onClick={() => handleBatchInteractionStatusUpdate(st)}
                    className="px-2 py-1 text-[10px] font-extrabold rounded-lg bg-slate-900 border border-slate-800 hover:bg-slate-800 hover:border-slate-700 hover:text-white transition"
                  >
                    {st}
                  </button>
                ))}

                <div className="w-px h-6 bg-slate-800 mx-1 hidden md:block" />

                <button
                  onClick={handleBatchInteractionDelete}
                  className="px-3 py-1.5 text-[10.5px] font-extrabold bg-red-950 text-red-400 hover:bg-red-900 hover:text-white rounded-lg border border-red-900/50 transition flex items-center gap-1"
                >
                  <Trash2 className="w-3 h-3" /> Delete
                </button>
              </div>
            )}

            {/* Contacts Batch Panel Actions */}
            {activeTab === "contacts" && (
              <div className="flex flex-wrap items-center gap-2">
                {/* Role Updater Form input */}
                <form
                  onSubmit={(e) => {
                    e.preventDefault();
                    const fd = new FormData(e.currentTarget);
                    const rl = fd.get("new-role") as string;
                    if (rl) {
                      handleBatchContactRoleUpdate(rl);
                      e.currentTarget.reset();
                    }
                  }}
                  className="flex items-center bg-slate-900 border border-slate-850 rounded-lg p-1"
                >
                  <input
                    name="new-role"
                    type="text"
                    required
                    placeholder="New Role (e.g. CMO)"
                    className="bg-transparent px-2 py-0.5 text-[10.5px] outline-none text-white placeholder-slate-500 w-36 font-semibold"
                  />
                  <button
                    type="submit"
                    className="px-2 py-1 text-[9.5px] font-extrabold bg-emerald-600 hover:bg-emerald-500 text-white rounded font-sans transition"
                  >
                    Set Role
                  </button>
                </form>

                {/* Company Updater Form input */}
                <form
                  onSubmit={(e) => {
                    e.preventDefault();
                    const fd = new FormData(e.currentTarget);
                    const cmp = fd.get("new-company") as string;
                    if (cmp) {
                      handleBatchContactCompanyUpdate(cmp);
                      e.currentTarget.reset();
                    }
                  }}
                  className="flex items-center bg-slate-900 border border-slate-850 rounded-lg p-1"
                >
                  <input
                    name="new-company"
                    type="text"
                    required
                    placeholder="New Company"
                    className="bg-transparent px-2 py-0.5 text-[10.5px] outline-none text-white placeholder-slate-500 w-36 font-semibold"
                  />
                  <button
                    type="submit"
                    className="px-2 py-1 text-[9.5px] font-extrabold bg-blue-600 hover:bg-blue-500 text-white rounded font-sans transition"
                  >
                    Set Org
                  </button>
                </form>

                <div className="w-px h-6 bg-slate-800 mx-1 hidden md:block" />

                <button
                  onClick={handleBatchContactDelete}
                  className="px-3 py-1.5 text-[10.5px] font-extrabold bg-red-950 text-red-400 hover:bg-red-900 hover:text-white rounded-lg border border-red-900/50 transition flex items-center gap-1"
                >
                  <Trash2 className="w-3 h-3" /> Delete
                </button>
              </div>
            )}

            <div className="flex items-center gap-2 border-t md:border-t-0 border-slate-800 pt-2.5 md:pt-0 w-full md:w-auto justify-end shrink-0">
              <button
                onClick={() => {
                  setSelectedInteractionIds([]);
                  setSelectedContactIds([]);
                }}
                className="text-slate-400 hover:text-white text-[11px] font-semibold font-sans px-3 py-1.5"
              >
                Dismiss
              </button>
            </div>
          </motion.div>
        )}
      </AnimatePresence>

    </div>
  );
}
