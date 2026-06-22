import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrmService } from '../../core/services/crm.service';
import { Contact } from '../../core/models/crm.models';
import { LucideAngularModule, UserPlus, MoreVertical, Mail, Phone, Search } from 'lucide-angular';

@Component({
  selector: 'app-contact-list',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="flex justify-between items-end mb-8">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">Contacts</h1>
        <p class="text-slate-500 text-sm">Manage your relationships and network.</p>
      </div>
      <div class="flex gap-3">
        <button class="btn-secondary flex items-center text-sm px-4">
          <lucide-icon [name]="Search" size="16" class="mr-2"></lucide-icon>
          Filter
        </button>
        <button class="btn-primary flex items-center text-sm px-4">
          <lucide-icon [name]="UserPlus" size="16" class="mr-2"></lucide-icon>
          Add Contact
        </button>
      </div>
    </div>

    <div class="card overflow-hidden">
      <table class="w-full text-left border-collapse">
        <thead class="bg-slate-50 border-b border-slate-200 text-[11px] font-bold text-slate-500 uppercase tracking-widest text-nowrap">
          <tr>
            <th class="px-6 py-4">Contact</th>
            <th class="px-6 py-4">Contact Info</th>
            <th class="px-6 py-4">Rating</th>
            <th class="px-6 py-4">Tags</th>
            <th class="px-6 py-4 text-right">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-100 text-sm">
          @for (contact of contacts; track contact.contactId) {
            <tr class="hover:bg-slate-50 transition-colors group cursor-pointer">
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-10 h-10 rounded-full bg-blue-100 flex items-center justify-center text-blue-700 font-bold">
                    {{ contact.firstName[0] }}{{ contact.lastName[0] }}
                  </div>
                  <div>
                    <p class="font-bold text-slate-800">{{ contact.firstName }} {{ contact.lastName }}</p>
                    <p class="text-xs text-slate-400 italic">Added {{ contact.createdAt | date }}</p>
                  </div>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="space-y-1">
                  <div class="flex items-center text-slate-600 gap-2">
                    <lucide-icon [name]="Mail" size="12" class="text-slate-400"></lucide-icon>
                    {{ contact.email }}
                  </div>
                  <div class="flex items-center text-slate-600 gap-2">
                    <lucide-icon [name]="Phone" size="12" class="text-slate-400"></lucide-icon>
                    {{ contact.phone }}
                  </div>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="flex gap-0.5">
                  @for (star of [1,2,3,4,5]; track star) {
                    <div class="w-2.5 h-2.5 rounded-full" [ngClass]="star <= contact.rating ? 'bg-amber-400' : 'bg-slate-200'"></div>
                  }
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="flex flex-wrap gap-1">
                  @for (tag of (contact.contactTags?.split(',') || []); track tag) {
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
export class ContactListComponent implements OnInit {
  private crmService = inject(CrmService);
  contacts: Contact[] = [];

  UserPlus = UserPlus;
  MoreVertical = MoreVertical;
  Mail = Mail;
  Phone = Phone;
  Search = Search;

  ngOnInit(): void {
    this.crmService.getContacts().subscribe(res => {
      this.contacts = res.items;
    });
  }
}
