import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { CrmService } from '../../core/services/crm.service';
import { User } from '../../core/models/crm.models';
import { LucideAngularModule, UserPlus, MoreVertical, ShieldCheck, ShieldAlert } from 'lucide-angular';

@Component({
  selector: 'app-user-list',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  template: `
    <div class="flex justify-between items-end mb-8">
      <div>
        <h1 class="text-2xl font-bold text-slate-800">User Management</h1>
        <p class="text-slate-500 text-sm">Manage system access and assign roles.</p>
      </div>
      <button class="btn-primary flex items-center text-sm">
        <lucide-icon [name]="UserPlus" size="16" class="mr-2"></lucide-icon>
        Add User
      </button>
    </div>

    <div class="card overflow-hidden">
      <table class="w-full text-left border-collapse">
        <thead class="bg-slate-50 border-b border-slate-200 text-[11px] font-bold text-slate-500 uppercase tracking-widest text-nowrap">
          <tr>
            <th class="px-6 py-4">User</th>
            <th class="px-6 py-4">Role</th>
            <th class="px-6 py-4">Status</th>
            <th class="px-6 py-4 text-right">Actions</th>
          </tr>
        </thead>
        <tbody class="divide-y divide-slate-100 text-sm">
          @for (user of users; track user.userId) {
            <tr class="hover:bg-slate-50 transition-colors group">
              <td class="px-6 py-4">
                <div class="flex items-center gap-3">
                  <div class="w-8 h-8 rounded-full bg-slate-100 flex items-center justify-center text-slate-600 font-bold text-xs">
                    {{ user.username[0].toUpperCase() }}
                  </div>
                  <span class="font-bold text-slate-800">{{ user.username }}</span>
                </div>
              </td>
              <td class="px-6 py-4">
                <div class="flex items-center gap-2">
                  <lucide-icon [name]="user.roleName === 'Administrator' ? ShieldCheck : ShieldAlert" size="14" class="text-slate-400"></lucide-icon>
                  <span class="text-slate-600 font-medium">{{ user.roleName }}</span>
                </div>
              </td>
              <td class="px-6 py-4">
                <span class="px-2 py-0.5 rounded-full text-[10px] font-bold uppercase tracking-wider"
                      [ngClass]="user.isActive ? 'bg-emerald-50 text-emerald-600 border border-emerald-100' : 'bg-rose-50 text-rose-600 border border-rose-100'">
                  {{ user.isActive ? 'Active' : 'Disabled' }}
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
    </div>
  `
})
export class UserListComponent implements OnInit {
  private crmService = inject(CrmService);
  users: User[] = [];

  UserPlus = UserPlus;
  MoreVertical = MoreVertical;
  ShieldCheck = ShieldCheck;
  ShieldAlert = ShieldAlert;

  ngOnInit(): void {
    // Note: CrmService needs getUsers() method
    this.crmService.getUsers().subscribe(res => {
      this.users = res.items;
    });
  }
}
