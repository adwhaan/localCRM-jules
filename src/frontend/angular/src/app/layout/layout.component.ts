import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { CommonModule } from '@angular/common';
import { AuthService } from '../core/services/auth.service';
import {
  LayoutDashboard,
  Users,
  Building2,
  Handshake,
  MessageSquare,
  FileText,
  StickyNote,
  Settings,
  LogOut,
  Bell,
  Search,
  ChevronRight,
  ShieldCheck,
  Tags,
  ListRestart,
  Zap
} from 'lucide-angular';
import { LucideAngularModule } from 'lucide-angular';

@Component({
  selector: 'app-layout',
  standalone: true,
  imports: [CommonModule, RouterOutlet, RouterLink, RouterLinkActive, LucideAngularModule],
  template: `
    <div class="flex h-screen overflow-hidden">
      <!-- Sidebar -->
      <aside class="w-64 bg-slate-900 flex-shrink-0 flex flex-col border-r border-slate-800">
        <div class="p-6 border-b border-slate-800 flex items-center gap-3">
          <div class="w-8 h-8 bg-primary-600 rounded flex items-center justify-center">
            <lucide-icon [name]="Zap" size="18" class="text-white"></lucide-icon>
          </div>
          <div class="overflow-hidden text-white font-bold truncate">Enterprise Vault</div>
        </div>

        <nav class="flex-grow py-4 overflow-y-auto custom-scrollbar">
          <div class="px-6 py-2">
            <span class="text-slate-500 text-[10px] font-bold uppercase tracking-wider">Main</span>
          </div>
          <a routerLink="/dashboard" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="LayoutDashboard" size="18" class="mr-3"></lucide-icon>
            Dashboard
          </a>

          <div class="px-6 py-2 mt-4">
            <span class="text-slate-500 text-[10px] font-bold uppercase tracking-wider">Workspace</span>
          </div>
          <a routerLink="/contacts" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="Users" size="18" class="mr-3"></lucide-icon>
            Contacts
          </a>
          <a routerLink="/companies" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="Building2" size="18" class="mr-3"></lucide-icon>
            Companies
          </a>
          <a routerLink="/engagements" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="Handshake" size="18" class="mr-3"></lucide-icon>
            Engagements
          </a>

          <div class="px-6 py-2 mt-4">
            <span class="text-slate-500 text-[10px] font-bold uppercase tracking-wider">Activity</span>
          </div>
          <a routerLink="/interactions" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="MessageSquare" size="18" class="mr-3"></lucide-icon>
            Interactions
          </a>
          <a routerLink="/notes" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="StickyNote" size="18" class="mr-3"></lucide-icon>
            Notes
          </a>
          <a routerLink="/documents" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="FileText" size="18" class="mr-3"></lucide-icon>
            Documents
          </a>

          <div class="px-6 py-2 mt-4">
            <span class="text-slate-500 text-[10px] font-bold uppercase tracking-wider">System</span>
          </div>
          <a routerLink="/users" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="Users" size="18" class="mr-3"></lucide-icon>
            Users
          </a>
          <a routerLink="/roles" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="ShieldCheck" size="18" class="mr-3"></lucide-icon>
            Roles
          </a>
          <a routerLink="/settings" routerLinkActive="active-link" class="nav-link">
            <lucide-icon [name]="Settings" size="18" class="mr-3"></lucide-icon>
            Settings
          </a>
        </nav>

        <div class="p-4 border-t border-slate-800">
          <button (click)="logout()" class="flex items-center w-full px-4 py-2 text-slate-400 hover:text-white transition-colors text-sm rounded-lg hover:bg-slate-800">
            <lucide-icon [name]="LogOut" size="18" class="mr-3"></lucide-icon>
            Sign out
          </button>
        </div>
      </aside>

      <!-- Main Content -->
      <main class="flex-grow flex flex-col min-w-0 bg-slate-50">
        <!-- Header -->
        <header class="h-16 bg-white border-b border-slate-200 flex items-center px-8 flex-shrink-0">
          <div class="flex-grow flex items-center">
            <div class="hidden md:flex items-center bg-slate-100 rounded-lg px-3 py-1 w-96">
              <lucide-icon [name]="Search" size="14" class="text-slate-400 mr-2"></lucide-icon>
              <input type="text" placeholder="Search contacts, companies..." class="bg-transparent border-none outline-none text-sm w-full py-1 text-slate-700" />
            </div>
          </div>

          <div class="flex items-center gap-4">
            <div class="relative">
              <button class="p-2 text-slate-400 hover:text-slate-600 transition-colors">
                <lucide-icon [name]="Bell" size="20"></lucide-icon>
                <span class="absolute top-1.5 right-1.5 w-2 h-2 bg-red-500 border-2 border-white rounded-full"></span>
              </button>
            </div>
            <div class="flex items-center gap-3 pl-4 border-l border-slate-200">
              <div class="text-right hidden sm:block">
                <p class="text-xs font-bold text-slate-800">{{ currentUser?.username }}</p>
                <p class="text-[10px] text-slate-500 uppercase tracking-tighter">Administrator</p>
              </div>
              <div class="w-8 h-8 rounded-full bg-primary-100 border border-primary-200 flex items-center justify-center text-primary-700 text-xs font-bold uppercase">
                JD
              </div>
            </div>
          </div>
        </header>

        <!-- Content Area -->
        <div class="flex-grow overflow-y-auto custom-scrollbar p-8">
          <router-outlet></router-outlet>
        </div>
      </main>
    </div>
  `,
  styles: [`
    .nav-link {
      @apply flex items-center px-6 py-2.5 text-slate-400 hover:text-slate-50 transition-all text-sm relative;
    }
    .active-link {
      @apply bg-primary-600 text-white font-medium;
    }
    .custom-scrollbar::-webkit-scrollbar {
      width: 4px;
    }
    .custom-scrollbar::-webkit-scrollbar-track {
      background: transparent;
    }
    .custom-scrollbar::-webkit-scrollbar-thumb {
      @apply bg-slate-700 rounded-full;
    }
  `]
})
export class LayoutComponent {
  private authService = inject(AuthService);

  LayoutDashboard = LayoutDashboard;
  Users = Users;
  Building2 = Building2;
  Handshake = Handshake;
  MessageSquare = MessageSquare;
  FileText = FileText;
  StickyNote = StickyNote;
  Settings = Settings;
  LogOut = LogOut;
  Bell = Bell;
  Search = Search;
  ShieldCheck = ShieldCheck;
  Zap = Zap;

  get currentUser() {
    return this.authService.currentUser;
  }

  logout() {
    this.authService.logout();
    window.location.reload();
  }
}
