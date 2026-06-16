export interface PagedResult<T> {
    items: T[];
    totalCount: number;
    offset: number;
    limit: number;
}

export interface Company {
    companyId: number;
    companyRef: string;
    name: string;
    addressLine1?: string;
    addressLine2?: string;
    postalCode?: string;
    city: string;
    country?: string;
    phone?: string;
    website?: string;
    email?: string;
    companyTags?: string;
    companyType: string;
    branch?: string;
    size?: string;
    rating: number;
    createdAt: string;
}

export interface Contact {
    contactId: number;
    contactRef?: string;
    firstName: string;
    middleName?: string;
    lastName: string;
    phone?: string;
    email?: string;
    linkedInUrl?: string;
    birthdate?: string;
    contactTags?: string;
    rating: number;
    sex?: string;
    createdAt: string;
}

export interface Engagement {
    engagementId: number;
    engagementRef?: string;
    description?: string;
    engagementTags?: string;
    confidentiality?: string;
    engagementStatus: string;
    createdAt: string;
}

export interface Interaction {
    interactionId: number;
    interactionDate: string;
    interactionTime?: string;
    direction?: string;
    interactionType: string;
    subject: string;
    note?: string;
    state: string;
    isTask: boolean;
    createdAt: string;
}

export interface DashboardMetrics {
    totalContacts: number;
    totalCompanies: number;
    totalInteractions: number;
    totalEngagements: number;
    totalDocuments: number;
    totalNotes: number;
    recentInteractions: RecentInteraction[];
}

export interface RecentInteraction {
    id: number;
    type: string;
    subject: string;
    interactionDate: string;
    contactName: string;
    companyName: string;
    state: string;
}

export interface User {
    id: number;
    username: string;
    email: string;
    isActive: boolean;
    roles: string[];
}

export interface AuthResponse {
    token: string;
    refreshToken: string;
    username: string;
    roles: string[];
    mustChangePassword: boolean;
}
