import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrmService } from '../../core/services/crm.service';
import { Engagement } from '../../core/models/crm.models';
import { LucideAngularModule, Handshake, MoreVertical, ShieldAlert, Plus, Filter } from 'lucide-angular';

@Component({
  selector: 'app-engagement-list',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="flex justify-between items-end mb-8">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">Engagements</h1>
        <p class="text-slate-500 text-sm">Track deals, projects, and active mandates.</p>
      </div>
      <div class="flex gap-3">
        <button class="btn-secondary flex items-center text-sm px-4">
          <lucide-icon [name]="Filter" size="16" class="mr-2"></lucide-icon>
          Filter
        </button>
        <button class="btn-primary flex items-center text-sm px-4">
          <lucide-icon [name]="Plus" size="16" class="mr-2"></lucide-icon>
          New Engagement
        </button>
      </div>
    </div>

    <div class="card overflow-hidden">
      <table class="w-full text-left border-collapse">
        <thead class="bg-slate-50 border-b border-slate-200 text-[11px] font-bold text-slate-500 uppercase tracking-widest text-nowrap">
          <tr>
            <th class="px-6 py-4">Engagement</th>
            <th class="px-6 py-4">Description</th>
            <th class="px-6 py-4">Status</th>
            <th class="px-6 py-4">Tags</th>
            <th class="px-6 py-4 text-right">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-100 text-sm">
          @for (engagement of engagements; track engagement.engagementId) {
            <tr class="hover:bg-slate-50 transition-colors group">
              <td class="px-6 py-4 text-nowrap">
                <p class="font-bold text-slate-800">{{ engagement.engagementRef }}</p>
                <div class="flex items-center text-[10px] text-slate-400 font-medium gap-1 uppercase tracking-tighter">
                  <lucide-icon [name]="ShieldAlert" size="10"></lucide-icon>
                  {{ engagement.confidentiality || 'Standard' }}
                </div>
              </td>
              <td class="px-6 py-4">
                <p class="text-slate-600 line-clamp-1 max-w-xs">{{ engagement.description }}</p>
              </td>
              <td class="px-6 py-4">
                <span class="px-2 py-0.5 rounded-md text-[10px] font-bold uppercase border" [ngClass]="getStatusClass(engagement.engagementStatus)">
                  {{ engagement.engagementStatus }}
                </span>
              </td>
              <td class="px-6 py-4">
                <div class="flex flex-wrap gap-1">
                  @for (tag of (engagement.engagementTags?.split(',') || []); track tag) {
                    <span class="px-2 py-0.5 rounded-full bg-slate-100 text-slate-600 text-[10px] font-medium border border-slate-200 uppercase tracking-tighter">
                      {{ tag.trim() }}
                    </span>
                  }
                </div>
              </td>
              <td class="px-6 py-4 text-right">
                <button class="p-2 text-slate-400 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition-all opacity-0 group-hover:opacity-100">
                  <lucide-icon [name]="MoreVertical" size="18"></lucide-icon>
                </button>
              </td>
            </tr>
          }
        </tbody>
      </table>
    </div>
  `
})
export class EngagementListComponent implements OnInit {
  private crmService = inject(CrmService);
  engagements: Engagement[] = [];

  Handshake = Handshake;
  MoreVertical = MoreVertical;
  ShieldAlert = ShieldAlert;
  Plus = Plus;
  Filter = Filter;

  ngOnInit(): void {
    this.crmService.getEngagements().subscribe(res => {
      this.engagements = res.items;
    });
  }

  getStatusClass(status: string) {
    switch (status.toLowerCase()) {
      case 'active': return 'bg-emerald-50 text-emerald-600 border-emerald-100';
      case 'pending': return 'bg-amber-50 text-amber-600 border-amber-100';
      case 'on hold': return 'bg-rose-50 text-rose-600 border-rose-100';
      default: return 'bg-slate-50 text-slate-600 border-slate-100';
    }
  }
}
