export interface RentRecord {
  id: string;
  tenantId: string;
  propertyId: string;
  billingYear: number;
  billingMonth: number;
  dueDate: string;
  expectedAmount: number;
  paidAmount: number;
  status: 'Unpaid' | 'Partial' | 'Paid';
  paidOnUtc?: string;
  notes?: string;
  tenantName: string;
  propertyName: string;
}

export interface DashboardSummary {
  totalTenants: number;
  totalDue: number;
  collectedAmount: number;
  pendingAmount: number;
}
