export interface Tenant {
  id: string;
  propertyId: string;
  fullName: string;
  phone: string;
  email?: string;
  roomOrBed: string;
  monthlyRent: number;
  rentDueDay: number;
  isActive: boolean;
}

export interface TenantPayload {
  fullName: string;
  phone: string;
  email?: string;
  roomOrBed: string;
  monthlyRent: number;
  rentDueDay: number;
  moveInDate?: string;
  isActive?: boolean;
}
