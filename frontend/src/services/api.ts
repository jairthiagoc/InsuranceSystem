import axios, { AxiosInstance, AxiosResponse } from 'axios';
import { 
  Proposal, 
  Contract, 
  CreateProposalRequest, 
  UpdateProposalStatusRequest, 
  CreateContractRequest,
  PaginatedResult,
  GetProposalsRequest,
  ApiResponse 
} from '../types';

// Configuração base do axios
const createApiInstance = (baseURL: string): AxiosInstance => {
  const instance = axios.create({
    baseURL,
    timeout: 10000,
    headers: {
      'Content-Type': 'application/json',
    },
  });

  // Interceptor para tratamento de erros
  instance.interceptors.response.use(
    (response) => response,
    (error) => {
      console.error('API Error:', error);
      return Promise.reject(error);
    }
  );

  return instance;
};

// Instâncias para cada microserviço
const proposalApi = createApiInstance('http://localhost:7001/api');
const contractApi = createApiInstance('http://localhost:7002/api');

// Serviço de Propostas
export const proposalService = {
  // Criar proposta
  createProposal: async (data: CreateProposalRequest): Promise<Proposal> => {
    const response: AxiosResponse<Proposal> = await proposalApi.post('/proposals', data);
    return response.data;
  },

  // Listar propostas (método antigo - mantido para compatibilidade)
  getProposals: async (): Promise<Proposal[]> => {
    const response: AxiosResponse<Proposal[]> = await proposalApi.get('/proposals');
    return response.data;
  },

  // Listar propostas com paginação
  getProposalsPaginated: async (params: GetProposalsRequest = {}): Promise<PaginatedResult<Proposal>> => {
    const queryParams = new URLSearchParams();
    
    if (params.pageNumber) queryParams.append('pageNumber', params.pageNumber.toString());
    if (params.pageSize) queryParams.append('pageSize', params.pageSize.toString());
    if (params.searchTerm) queryParams.append('searchTerm', params.searchTerm);
    if (params.statusFilter) queryParams.append('statusFilter', params.statusFilter);
    
    const response: AxiosResponse<PaginatedResult<Proposal>> = await proposalApi.get(`/proposals?${queryParams}`);
    return response.data;
  },

  // Obter proposta por ID
  getProposalById: async (id: string): Promise<Proposal> => {
    const response: AxiosResponse<Proposal> = await proposalApi.get(`/proposals/${id}`);
    return response.data;
  },

  // Atualizar status da proposta
  updateProposalStatus: async (id: string, data: UpdateProposalStatusRequest): Promise<Proposal> => {
    const response: AxiosResponse<Proposal> = await proposalApi.put(`/proposals/${id}/status`, data);
    return response.data;
  },

  // Obter propostas por status
  getProposalsByStatus: async (status: string): Promise<Proposal[]> => {
    const response: AxiosResponse<Proposal[]> = await proposalApi.get(`/proposals?status=${status}`);
    return response.data;
  },

  // Atualizar proposta
  updateProposal: async (id: string, data: CreateProposalRequest): Promise<Proposal> => {
    const response: AxiosResponse<Proposal> = await proposalApi.put(`/proposals/${id}`, data);
    return response.data;
  }
};

// Serviço de Contratos
export const contractService = {
  // Criar contrato
  createContract: async (data: CreateContractRequest): Promise<Contract> => {
    const response: AxiosResponse<Contract> = await contractApi.post('/contracts', data);
    return response.data;
  },

  // Listar contratos
  getContracts: async (): Promise<Contract[]> => {
    const response: AxiosResponse<Contract[]> = await contractApi.get('/contracts');
    return response.data;
  },

  // Obter contrato por ID
  getContractById: async (id: string): Promise<Contract> => {
    const response: AxiosResponse<Contract> = await contractApi.get(`/contracts/${id}`);
    return response.data;
  },

  // Obter contrato por proposta
  getContractByProposalId: async (proposalId: string): Promise<Contract | null> => {
    try {
      const response: AxiosResponse<Contract> = await contractApi.get(`/contracts/proposal/${proposalId}`);
      return response.data;
    } catch (error) {
      if (axios.isAxiosError(error) && error.response?.status === 404) {
        return null;
      }
      throw error;
    }
  }
};

// Serviço de Dashboard
export const dashboardService = {
  // Obter estatísticas do dashboard
  getStats: async () => {
    try {
      const [proposals, contracts] = await Promise.all([
        proposalService.getProposals(),
        contractService.getContracts()
      ]);

      const totalProposals = proposals.length;
      const pendingProposals = proposals.filter(p => p.status === 'UnderReview').length;
      const approvedProposals = proposals.filter(p => p.status === 'Approved').length;
      const rejectedProposals = proposals.filter(p => p.status === 'Rejected').length;
      const totalContracts = contracts.length;
      const totalRevenue = contracts.reduce((sum, contract) => sum + contract.premiumAmount, 0);

      return {
        totalProposals,
        pendingProposals,
        approvedProposals,
        rejectedProposals,
        totalContracts,
        totalRevenue
      };
    } catch (error) {
      console.error('Error fetching dashboard stats:', error);
      return {
        totalProposals: 0,
        pendingProposals: 0,
        approvedProposals: 0,
        rejectedProposals: 0,
        totalContracts: 0,
        totalRevenue: 0
      };
    }
  }
};

export default {
  proposalService,
  contractService,
  dashboardService
}; 