import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PagedResult, Company, Contact, Engagement, Interaction, DashboardMetrics, User } from '../models/crm.models';

@Injectable({
  providedIn: 'root'
})
export class CrmService {
  private http = inject(HttpClient);

  getDashboardMetrics(): Observable<DashboardMetrics> {
    return this.http.get<DashboardMetrics>('api/dashboard');
  }

  getCompanies(offset = 0, limit = 10): Observable<PagedResult<Company>> {
    return this.http.get<PagedResult<Company>>(`api/companies?offset=${offset}&limit=${limit}`);
  }

  getContacts(offset = 0, limit = 10): Observable<PagedResult<Contact>> {
    return this.http.get<PagedResult<Contact>>(`api/contacts?offset=${offset}&limit=${limit}`);
  }

  getEngagements(offset = 0, limit = 10): Observable<PagedResult<Engagement>> {
    return this.http.get<PagedResult<Engagement>>(`api/engagements?offset=${offset}&limit=${limit}`);
  }

  getInteractions(offset = 0, limit = 10): Observable<PagedResult<Interaction>> {
    return this.http.get<PagedResult<Interaction>>(`api/interactions?offset=${offset}&limit=${limit}`);
  }

  getUsers(offset = 0, limit = 10): Observable<PagedResult<User>> {
    return this.http.get<PagedResult<User>>(`api/users?offset=${offset}&limit=${limit}`);
  }
}
