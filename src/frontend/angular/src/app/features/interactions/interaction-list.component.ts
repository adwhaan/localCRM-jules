import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrmService } from '../../core/services/crm.service';
import { Interaction } from '../../core/models/crm.models';
import { LucideAngularModule, MessageSquare, MoreVertical, Plus, Filter, LayoutList, CalendarDays } from 'lucide-angular';

@Component({
  selector: 'app-interaction-list',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="flex justify-between items-end mb-8">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">Interactions</h1>
        <p class="text-slate-500 text-sm">History of all touchpoints and communications.</p>
      </div>
      <div class="flex items-center gap-4">
        <div class="bg-white border border-slate-200 rounded-lg p-1 flex shadow-sm">
          <button class="px-3 py-1.5 rounded-md flex items-center text-xs font-bold transition-all"
                  [class.bg-primary-600]="viewMode === 'list'" [class.text-white]="viewMode === 'list'"
                  [class.text-slate-500]="viewMode !== 'list'" (click)="viewMode = 'list'">
            <lucide-icon [name]="LayoutList" size="14" class="mr-2"></lucide-icon>
            List
          </button>
          <button class="px-3 py-1.5 rounded-md flex items-center text-xs font-bold transition-all"
                  [class.bg-primary-600]="viewMode === 'timeline'" [class.text-white]="viewMode === 'timeline'"
                  [class.text-slate-500]="viewMode !== 'timeline'" (click)="viewMode = 'timeline'">
            <lucide-icon [name]="CalendarDays" size="14" class="mr-2"></lucide-icon>
            Activity
          </button>
        </div>

        <button class="btn-primary flex items-center text-sm px-4">
          <lucide-icon [name]="Plus" size="16" class="mr-2"></lucide-icon>
          Log Interaction
        </button>
      </div>
    </div>

    <div class="card overflow-hidden">
      @if (viewMode === 'list') {
        <table class="w-full text-left border-collapse">
          <thead class="bg-slate-50 border-b border-slate-200 text-[11px] font-bold text-slate-500 uppercase tracking-widest text-nowrap">
            <tr>
              <th class="px-6 py-4">Date</th>
              <th class="px-6 py-4">Subject</th>
              <th class="px-6 py-4">Type</th>
              <th class="px-6 py-4">State</th>
              <th class="px-6 py-4 text-right">Actions</th>
            </tr>
          </thead>
          <tbody class="divide-y divide-slate-100 text-sm">
            @for (interaction of interactions; track interaction.interactionId) {
              <tr class="hover:bg-slate-50 transition-colors group">
                <td class="px-6 py-4 text-nowrap">
                  <p class="font-bold text-slate-800">{{ interaction.interactionDate | date:'mediumDate' }}</p>
                  <p class="text-[10px] text-slate-400 font-medium uppercase tracking-tighter">{{ interaction.interactionTime }}</p>
                </td>
                <td class="px-6 py-4">
                  <p class="text-slate-700 font-medium">{{ interaction.subject }}</p>
                </td>
                <td class="px-6 py-4">
                  <span class="px-2 py-0.5 rounded-md text-[10px] font-bold uppercase border" [ngClass]="getTypeClass(interaction.interactionType)">
                    {{ interaction.interactionType }}
                  </span>
                </td>
                <td class="px-6 py-4">
                  <span class="px-2 py-0.5 rounded-md bg-slate-100 text-slate-600 text-[10px] font-bold uppercase">
                    {{ interaction.state }}
                  </span>
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
      } @else {
        <div class="p-12 text-center">
            <p class="text-slate-400 italic">Timeline view is active. Implementation details following the canonical design.</p>
        </div>
      }
    </div>
  `
})
export class InteractionListComponent implements OnInit {
  private crmService = inject(CrmService);
  interactions: Interaction[] = [];
  viewMode: 'list' | 'timeline' = 'list';

  MessageSquare = MessageSquare;
  MoreVertical = MoreVertical;
  Plus = Plus;
  Filter = Filter;
  LayoutList = LayoutList;
  CalendarDays = CalendarDays;

  ngOnInit(): void {
    this.crmService.getInteractions().subscribe(res => {
      this.interactions = res.items;
    });
  }

  getTypeClass(type: string) {
    switch (type.toLowerCase()) {
      case 'call': return 'bg-blue-50 text-blue-600 border-blue-100';
      case 'email': return 'bg-indigo-50 text-indigo-600 border-indigo-100';
      case 'meeting': return 'bg-emerald-50 text-emerald-600 border-emerald-100';
      case 'note': return 'bg-amber-50 text-amber-600 border-amber-100';
      default: return 'bg-slate-50 text-slate-600 border-slate-100';
    }
  }
}
