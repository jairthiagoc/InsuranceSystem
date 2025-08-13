// Tipos para o sistema de seguros

export interface Proposal {
  id: string;
  customerName: string;
  customerEmail: string;
  insuranceType: string;
  coverageAmount: number;
  premiumAmount: number;
  status: ProposalStatus;
  createdAt: string;
  updatedAt?: string;
  rejectionReason?: string;
}

export interface Contract {
  id: string;
  proposalId: string;
  contractNumber: string;
  premiumAmount: number;
  contractDate: string;
  createdAt: string;
  proposal?: Proposal;
}

export enum ProposalStatus {
  UnderReview = 'UnderReview',  // Em Análise
  Approved = 'Approved',        // Aprovada
  Rejected = 'Rejected'         // Rejeitada
}

export interface CreateProposalRequest {
  customerName: string;
  customerEmail: string;
  insuranceType: string;
  coverageAmount: number;
  premiumAmount: number;
}

export interface UpdateProposalStatusRequest {
  status: ProposalStatus;
  rejectionReason?: string;
}

export interface CreateContractRequest {
  proposalId: string;
}

export interface PaginatedResult<T> {
  items: T[];
  totalItems: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
}

export interface GetProposalsRequest {
  pageNumber?: number;
  pageSize?: number;
  searchTerm?: string;
  statusFilter?: string;
}

export interface ApiResponse<T> {
  data: T;
  success: boolean;
  message?: string;
}

export interface WebSocketEvent {
  type: 'proposal_created' | 'proposal_updated' | 'contract_created' | 'status_changed';
  data: any;
  timestamp: string;
}

export interface InsuranceType {
  id: string;
  name: string;
  description: string;
  minCoverage: number;
  maxCoverage: number;
  basePremiumRate: number;
}

export interface DashboardStats {
  totalProposals: number;
  pendingProposals: number;
  approvedProposals: number;
  rejectedProposals: number;
  totalContracts: number;
  totalRevenue: number;
} 