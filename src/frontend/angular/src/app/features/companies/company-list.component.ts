import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrmService } from '../../core/services/crm.service';
import { Company } from '../../core/models/crm.models';
import { LucideAngularModule, Building2, MoreVertical, Plus, MapPin, Search } from 'lucide-angular';

@Component({
  selector: 'app-company-list',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="flex justify-between items-end mb-8">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">Companies</h1>
        <p class="text-slate-500 text-sm">Manage your client organizations and partners.</p>
      </div>
      <div class="flex gap-3">
        <button class="btn-secondary flex items-center text-sm px-4">
          <lucide-icon [name]="Search" size="16" class="mr-2"></lucide-icon>
          Filter
        </button>
        <button class="btn-primary flex items-center text-sm px-4">
          <lucide-icon [name]="Plus" size="16" class="mr-2"></lucide-icon>
          Add Company
        </button>
      </div>
    </div>

    <div class="card overflow-hidden">
      <table class="w-full text-left border-collapse">
        <thead class="bg-slate-50 border-b border-slate-200 text-[11px] font-bold text-slate-500 uppercase tracking-widest text-nowrap">
          <tr>
            <th class="px-6 py-4">Company</th>
            <th class="px-6 py-4">Reference</th>
            <th class="px-6 py-4">Location</th>
            <th class="px-6 py-4">Rating</th>
            <th class="px-6 py-4 text-right">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-100 text-sm">
          @for (company of companies; track company.companyId) {
            <tr class="hover:bg-slate-50 transition-colors group">
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-xl bg-indigo-100 flex items-center justify-center text-indigo-700">
                    <lucide-icon [name]="Building2" size="20"></lucide-icon>
                  </div>
                  <div>
                    <p class="font-bold text-slate-800">{{ company.name }}</p>
                    <p class="text-[10px] text-slate-400 font-bold uppercase tracking-wider">{{ company.companyType }}</p>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4">
                <span class="font-mono text-xs font-bold text-slate-500 bg-slate-100 px-2 py-0.5 rounded">{{ company.companyRef }}</span>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center text-slate-600 gap-1.5 font-medium">
                  <lucide-icon [name]="MapPin" size="14" class="text-slate-400"></lucide-icon>
                  {{ company.city }}, {{ company.country }}
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="flex gap-0.5">
                  @for (star of [1,2,3,4,5]; track star) {
                    <div class="w-2.5 h-2.5 rounded-full" [ngClass]="star <= company.rating ? 'bg-amber-400' : 'bg-slate-200'"></div>
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
export class CompanyListComponent implements OnInit {
  private crmService = inject(CrmService);
  companies: Company[] = [];

  Building2 = Building2;
  MoreVertical = MoreVertical;
  Plus = Plus;
  MapPin = MapPin;
  Search = Search;

  ngOnInit(): void {
    this.crmService.getCompanies().subscribe(res => {
      this.companies = res.items;
    });
  }
}
