import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrmService } from '../../core/services/crm.service';
import { DashboardMetrics, RecentInteraction } from '../../core/models/crm.models';
<<<<<<< HEAD
import {
  Users,
  Building2,
  Handshake,
  MessageSquare,
  FileText,
=======
import {
  Users,
  Building2,
  Handshake,
  MessageSquare,
  FileText,
>>>>>>> origin/main
  StickyNote,
  ArrowUpRight,
  History,
  MoreVertical,
  Plus
} from 'lucide-angular';
import { LucideAngularModule } from 'lucide-angular';
import { formatDistanceToNow } from 'date-fns';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="mb-8">
      <h1 class="text-2xl font-bold text-slate-800">Welcome back,</h1>
      <p class="text-slate-500">Here's what's happening with your workspace today.</p>
    </div>

    <!-- Stats Grid -->
    <div class="grid grid-cols-2 md:grid-cols-3 lg:grid-cols-6 gap-4 mb-8">
      @for (stat of stats; track stat.label) {
        <div class="card p-4 border-l-4" [style.border-left-color]="stat.color">
          <p class="text-[10px] font-bold text-slate-500 uppercase tracking-wider mb-1">{{ stat.label }}</p>
          <div class="flex items-end justify-between">
            <h3 class="text-2xl font-bold text-slate-800">{{ stat.value }}</h3>
          </div>
        </div>
      }
    </div>

    <div class="grid grid-cols-1 lg:grid-cols-3 gap-8">
      <!-- Activity Feed -->
      <div class="lg:col-span-2 card p-6">
        <div class="flex items-center justify-between mb-6">
          <h2 class="text-lg font-bold text-slate-800 flex items-center">
            <lucide-icon [name]="History" size="18" class="mr-2 text-slate-400"></lucide-icon>
            Active Interactions Feed
          </h2>
          <button class="text-sm font-medium text-primary-600 hover:text-primary-700">View All</button>
        </div>

        <div class="space-y-6 relative before:absolute before:inset-0 before:left-3.5 before:w-0.5 before:bg-slate-100">
          @for (item of metrics?.recentInteractions; track item.id) {
            <div class="relative pl-10">
              <div class="absolute left-0 top-1 w-8 h-8 rounded-full border-4 border-white flex items-center justify-center z-10" [ngClass]="getIconBg(item.type)">
                <lucide-icon [name]="getIcon(item.type)" size="12" class="text-white"></lucide-icon>
              </div>
<<<<<<< HEAD

=======

>>>>>>> origin/main
              <div class="card p-4 hover:border-primary-300 transition-colors cursor-pointer group">
                <div class="flex justify-between items-start mb-2">
                  <h4 class="font-bold text-slate-800 text-sm group-hover:text-primary-600 transition-colors">{{ item.subject }}</h4>
                  <span class="text-[10px] text-slate-400 font-medium uppercase tracking-tighter">{{ formatTime(item.interactionDate) }}</span>
                </div>
<<<<<<< HEAD

=======

>>>>>>> origin/main
                <div class="flex items-center gap-2 mb-3">
                  <span class="px-2 py-0.5 rounded text-[10px] font-bold uppercase tracking-wide border" [ngClass]="getTypeClass(item.type)">
                    {{ item.type }}
                  </span>
                  <span class="px-2 py-0.5 rounded bg-slate-100 text-slate-600 text-[10px] font-bold uppercase tracking-wide">
                    {{ item.state }}
                  </span>
                </div>

                <div class="flex items-center gap-4 text-slate-500">
                  @if (item.contactName) {
                    <div class="flex items-center text-[11px]">
                      <lucide-icon [name]="Users" size="12" class="mr-1"></lucide-icon>
                      {{ item.contactName }}
                    </div>
                  }
                  @if (item.companyName) {
                    <div class="flex items-center text-[11px]">
                      <lucide-icon [name]="Building2" size="12" class="mr-1"></lucide-icon>
                      {{ item.companyName }}
                    </div>
                  }
                </div>
              </div>
            </div>
          } @empty {
            <div class="py-12 text-center">
              <p class="text-slate-400 text-sm italic">No recent activity found.</p>
            </div>
          }
        </div>
      </div>

      <!-- Quick Actions -->
      <div class="space-y-6">
        <div class="card p-6">
          <h2 class="text-lg font-bold text-slate-800 mb-6">Quick Actions</h2>
          <div class="grid grid-cols-1 gap-3">
            <button class="w-full flex items-center justify-between p-3 bg-primary-600 text-white rounded-xl font-medium hover:bg-primary-700 transition-colors group">
              <span class="flex items-center">
                <lucide-icon [name]="Plus" size="18" class="mr-3"></lucide-icon>
                New Interaction
              </span>
              <lucide-icon [name]="ArrowUpRight" size="14" class="opacity-50 group-hover:translate-x-0.5 group-hover:-translate-y-0.5 transition-transform"></lucide-icon>
            </button>
            <button class="w-full flex items-center justify-between p-3 border border-slate-200 text-slate-700 rounded-xl font-medium hover:bg-slate-50 transition-colors group text-sm">
              <span class="flex items-center">
                <lucide-icon [name]="Users" size="18" class="mr-3 text-slate-400"></lucide-icon>
                New Contact
              </span>
            </button>
            <button class="w-full flex items-center justify-between p-3 border border-slate-200 text-slate-700 rounded-xl font-medium hover:bg-slate-50 transition-colors group text-sm">
              <span class="flex items-center">
                <lucide-icon [name]="Building2" size="18" class="mr-3 text-slate-400"></lucide-icon>
                New Company
              </span>
            </button>
            <button class="w-full flex items-center justify-between p-3 border border-slate-200 text-slate-700 rounded-xl font-medium hover:bg-slate-50 transition-colors group text-sm">
              <span class="flex items-center">
                <lucide-icon [name]="Handshake" size="18" class="mr-3 text-slate-400"></lucide-icon>
                New Engagement
              </span>
            </button>
          </div>
        </div>

        <div class="card p-6">
          <h2 class="text-sm font-bold text-slate-800 mb-4 uppercase tracking-widest opacity-50">Workspace Health</h2>
          <div class="space-y-4">
            <div>
              <div class="flex justify-between text-xs mb-1.5">
                <span class="text-slate-500 font-medium">Profile Completion</span>
                <span class="text-slate-800 font-bold">85%</span>
              </div>
              <div class="w-full h-1.5 bg-slate-100 rounded-full overflow-hidden">
                <div class="h-full bg-primary-500 rounded-full" style="width: 85%"></div>
              </div>
            </div>
            <div>
              <div class="flex justify-between text-xs mb-1.5">
                <span class="text-slate-500 font-medium">Pending Tasks</span>
                <span class="text-slate-800 font-bold">12</span>
              </div>
              <div class="w-full h-1.5 bg-slate-100 rounded-full overflow-hidden">
                <div class="h-full bg-amber-500 rounded-full" style="width: 40%"></div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  `
})
export class DashboardComponent implements OnInit {
  private crmService = inject(CrmService);
<<<<<<< HEAD

=======

>>>>>>> origin/main
  metrics?: DashboardMetrics;
  stats: any[] = [];

  Users = Users;
  Building2 = Building2;
  Handshake = Handshake;
  History = History;
  Plus = Plus;
  ArrowUpRight = ArrowUpRight;

  ngOnInit(): void {
    this.crmService.getDashboardMetrics().subscribe(data => {
      this.metrics = data;
      this.stats = [
        { label: 'Contacts', value: data.totalContacts, color: '#3b82f6' },
        { label: 'Companies', value: data.totalCompanies, color: '#6366f1' },
        { label: 'Engagements', value: data.totalEngagements, color: '#10b981' },
        { label: 'Interactions', value: data.totalInteractions, color: '#f59e0b' },
        { label: 'Documents', value: data.totalDocuments, color: '#f43f5e' },
        { label: 'Notes', value: data.totalNotes, color: '#64748b' },
      ];
    });
  }

  formatTime(date: string) {
    try {
      return formatDistanceToNow(new Date(date), { addSuffix: true });
    } catch {
      return date;
    }
  }

  getIcon(type: string) {
    switch (type.toLowerCase()) {
      case 'call': return MessageSquare;
      case 'email': return FileText;
      case 'meeting': return Users;
      default: return StickyNote;
    }
  }

  getIconBg(type: string) {
    switch (type.toLowerCase()) {
      case 'call': return 'bg-blue-500';
      case 'email': return 'bg-indigo-500';
      case 'meeting': return 'bg-emerald-500';
      case 'note': return 'bg-amber-500';
      default: return 'bg-slate-500';
    }
  }

  getTypeClass(type: string) {
    switch (type.toLowerCase()) {
      case 'call': return 'text-blue-600 border-blue-200 bg-blue-50';
      case 'email': return 'text-indigo-600 border-indigo-200 bg-indigo-50';
      case 'meeting': return 'text-emerald-600 border-emerald-200 bg-emerald-50';
      case 'note': return 'text-amber-600 border-amber-200 bg-amber-50';
      default: return 'text-slate-600 border-slate-200 bg-slate-50';
    }
  }
}
