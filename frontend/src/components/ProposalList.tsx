import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  Plus, 
  Search, 
  Filter, 
  Eye, 
  Edit,
  CheckCircle,
  XCircle,
  Clock,
  FileText,
  ChevronLeft,
  ChevronRight
} from 'lucide-react';
import { proposalService } from '../services/api';
import { Proposal, ProposalStatus } from '../types';
import toast from 'react-hot-toast';
import ProposalForm from './ProposalForm';
import Modal from './Modal';
import ProposalDetails from './ProposalDetails';

const ProposalList: React.FC = () => {
  const [proposals, setProposals] = useState<Proposal[]>([]);
  const [loading, setLoading] = useState(true);
  const [searchTerm, setSearchTerm] = useState('');
  const [statusFilter, setStatusFilter] = useState<string>('all');
  const [isModalOpen, setIsModalOpen] = useState(false);
  const [selectedProposal, setSelectedProposal] = useState<Proposal | null>(null);
  const [isDetailsModalOpen, setIsDetailsModalOpen] = useState(false);
  const [isEditModalOpen, setIsEditModalOpen] = useState(false);
  
  // Estados para paginação
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage] = useState(10);
  const [totalItems, setTotalItems] = useState(0);
  const [totalPages, setTotalPages] = useState(0);

  useEffect(() => {
    loadProposals();
  }, [currentPage, searchTerm, statusFilter, itemsPerPage]);

  const loadProposals = async () => {
    try {
      setLoading(true);
      
      const params = {
        pageNumber: currentPage,
        pageSize: itemsPerPage,
        searchTerm: searchTerm || undefined,
        statusFilter: statusFilter !== 'all' ? statusFilter : undefined
      };
      
      const result = await proposalService.getProposalsPaginated(params);
      setProposals(result.items || []);
      setTotalItems(result.totalItems || 0);
      setTotalPages(result.totalPages || 0);
    } catch (error) {
      console.error('Error loading proposals:', error);
      toast.error('Erro ao carregar propostas');
      setProposals([]);
      setTotalItems(0);
      setTotalPages(0);
    } finally {
      setLoading(false);
    }
  };

  const handleViewDetails = async (proposalId: string) => {
    try {
      const proposal = await proposalService.getProposalById(proposalId);
      setSelectedProposal(proposal);
      setIsDetailsModalOpen(true);
    } catch (error) {
      console.error('Error loading proposal details:', error);
      toast.error('Erro ao carregar detalhes da proposta');
    }
  };

  const handleEditProposal = async (proposalId: string) => {
    try {
      const proposal = await proposalService.getProposalById(proposalId);
      
      // Verificar se a proposta pode ser editada
      if (proposal.status === 'Approved' || proposal.status === 'Rejected') {
        toast.error(
          proposal.status === 'Approved' 
            ? 'Proposta aprovada não pode ser editada'
            : 'Proposta rejeitada não pode ser editada'
        );
        return;
      }
      
      setSelectedProposal(proposal);
      setIsEditModalOpen(true);
    } catch (error) {
      console.error('Error loading proposal for edit:', error);
      toast.error('Erro ao carregar proposta para edição');
    }
  };

  const getStatusIcon = (status: ProposalStatus) => {
    switch (status) {
      case ProposalStatus.Approved:
        return <CheckCircle className="h-5 w-5 text-green-600" />;
      case ProposalStatus.Rejected:
        return <XCircle className="h-5 w-5 text-red-600" />;
      case ProposalStatus.UnderReview:
        return <Clock className="h-5 w-5 text-yellow-600" />;
      default:
        return <FileText className="h-5 w-5 text-gray-600" />;
    }
  };

  const getStatusColor = (status: ProposalStatus) => {
    switch (status) {
      case ProposalStatus.Approved:
        return 'bg-green-100 text-green-800';
      case ProposalStatus.Rejected:
        return 'bg-red-100 text-red-800';
      case ProposalStatus.UnderReview:
        return 'bg-yellow-100 text-yellow-800';
      default:
        return 'bg-gray-100 text-gray-800';
    }
  };

  const getStatusLabel = (status: ProposalStatus) => {
    switch (status) {
      case ProposalStatus.Approved:
        return 'Aprovada';
      case ProposalStatus.Rejected:
        return 'Rejeitada';
      case ProposalStatus.UnderReview:
        return 'Em Análise';
      default:
        return 'Em Análise';
    }
  };

  // Resetar página quando mudar filtros
  useEffect(() => {
    setCurrentPage(1);
  }, [searchTerm, statusFilter]);

  // Funções de navegação da paginação
  const goToPage = (page: number) => {
    setCurrentPage(page);
  };

  const goToPreviousPage = () => {
    setCurrentPage(prev => Math.max(prev - 1, 1));
  };

  const goToNextPage = () => {
    setCurrentPage(prev => Math.min(prev + 1, totalPages));
  };

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div className="flex justify-between items-center">
        <div>
          <h1 className="text-3xl font-bold text-gray-900">Propostas</h1>
          <p className="text-gray-600">Gerencie as propostas de seguro</p>
        </div>
        <button
          onClick={() => setIsModalOpen(true)}
          className="bg-primary-600 text-white px-4 py-2 rounded-md hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 flex items-center space-x-2"
        >
          <Plus className="h-4 w-4" />
          <span>Nova Proposta</span>
        </button>
      </div>

      {/* Filtros */}
      <div className="bg-white rounded-lg shadow-md p-6">
        <div className="flex flex-col md:flex-row gap-4">
          <div className="flex-1">
            <div className="relative">
              <Search className="absolute left-3 top-1/2 transform -translate-y-1/2 text-gray-400 h-4 w-4" />
              <input
                type="text"
                placeholder="Buscar por nome, email ou tipo..."
                value={searchTerm}
                onChange={(e) => setSearchTerm(e.target.value)}
                className="w-full pl-10 pr-4 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
              />
            </div>
          </div>
          <div className="flex items-center space-x-2">
            <Filter className="h-4 w-4 text-gray-400" />
            <select
              value={statusFilter}
              onChange={(e) => setStatusFilter(e.target.value)}
              className="px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500"
            >
              <option value="all">Todos os Status</option>
              <option value="UnderReview">Em Análise</option>
              <option value="Approved">Aprovada</option>
              <option value="Rejected">Rejeitada</option>
            </select>
          </div>
        </div>
      </div>

      {/* Lista de Propostas */}
      <div className="bg-white rounded-lg shadow-md overflow-hidden">
        <div className="overflow-x-auto">
          <table className="min-w-full divide-y divide-gray-200">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Cliente
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Tipo de Seguro
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Cobertura
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Prêmio
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Status
                </th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Data
                </th>
                <th className="px-6 py-3 text-right text-xs font-medium text-gray-500 uppercase tracking-wider">
                  Ações
                </th>
              </tr>
            </thead>
            <tbody className="bg-white divide-y divide-gray-200">
              {proposals?.map((proposal, index) => (
                <motion.tr
                  key={proposal.id}
                  initial={{ opacity: 0, y: 20 }}
                  animate={{ opacity: 1, y: 0 }}
                  transition={{ duration: 0.3, delay: index * 0.1 }}
                  className="hover:bg-gray-50"
                >
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div>
                      <div className="text-sm font-medium text-gray-900">
                        {proposal.customerName}
                      </div>
                      <div className="text-sm text-gray-500">
                        {proposal.customerEmail}
                      </div>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    {proposal.insuranceType}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    R$ {proposal.coverageAmount.toLocaleString('pt-BR')}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-900">
                    R$ {proposal.premiumAmount.toLocaleString('pt-BR')}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap">
                    <div className="flex items-center">
                      {getStatusIcon(proposal.status)}
                      <span className={`ml-2 inline-flex px-2 py-1 text-xs font-semibold rounded-full ${getStatusColor(proposal.status)}`}>
                        {getStatusLabel(proposal.status)}
                      </span>
                    </div>
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-sm text-gray-500">
                    {new Date(proposal.createdAt).toLocaleDateString('pt-BR')}
                  </td>
                  <td className="px-6 py-4 whitespace-nowrap text-right text-sm font-medium">
                    <div className="flex justify-end space-x-2">
                      <button 
                        onClick={() => handleViewDetails(proposal.id)}
                        className="text-primary-600 hover:text-primary-900"
                        title="Ver detalhes"
                      >
                        <Eye className="h-4 w-4" />
                      </button>
                      <button 
                        onClick={() => handleEditProposal(proposal.id)}
                        disabled={proposal.status === 'Approved' || proposal.status === 'Rejected'}
                        className={`${
                          proposal.status === 'Approved' || proposal.status === 'Rejected'
                            ? 'text-gray-300 cursor-not-allowed'
                            : 'text-gray-600 hover:text-gray-900'
                        }`}
                        title={
                          proposal.status === 'Approved' 
                            ? 'Proposta aprovada não pode ser editada'
                            : proposal.status === 'Rejected'
                            ? 'Proposta rejeitada não pode ser editada'
                            : 'Editar proposta'
                        }
                      >
                        <Edit className="h-4 w-4" />
                      </button>
                    </div>
                  </td>
                </motion.tr>
              ))}
            </tbody>
          </table>
        </div>
        
        {totalItems === 0 && (
          <div className="text-center py-12">
            <FileText className="mx-auto h-12 w-12 text-gray-400" />
            <h3 className="mt-2 text-sm font-medium text-gray-900">Nenhuma proposta encontrada</h3>
            <p className="mt-1 text-sm text-gray-500">
              {searchTerm || statusFilter !== 'all' 
                ? 'Tente ajustar os filtros de busca.' 
                : 'Comece criando uma nova proposta.'}
            </p>
          </div>
        )}

        {/* Paginação */}
        {totalItems > 0 && (
          <div className="bg-white px-4 py-3 flex items-center justify-between border-t border-gray-200 sm:px-6">
            <div className="flex-1 flex justify-between sm:hidden">
              <button
                onClick={goToPreviousPage}
                disabled={currentPage === 1}
                className="relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Anterior
              </button>
              <button
                onClick={goToNextPage}
                disabled={currentPage === totalPages}
                className="ml-3 relative inline-flex items-center px-4 py-2 border border-gray-300 text-sm font-medium rounded-md text-gray-700 bg-white hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
              >
                Próxima
              </button>
            </div>
            <div className="hidden sm:flex-1 sm:flex sm:items-center sm:justify-between">
              <div>
                <p className="text-sm text-gray-700">
                  Mostrando{' '}
                  <span className="font-medium">{(currentPage - 1) * itemsPerPage + 1}</span>
                  {' '}a{' '}
                  <span className="font-medium">
                    {Math.min(currentPage * itemsPerPage, totalItems)}
                  </span>
                  {' '}de{' '}
                  <span className="font-medium">{totalItems}</span>
                  {' '}resultados
                </p>
              </div>
              <div>
                <nav className="relative z-0 inline-flex rounded-md shadow-sm -space-x-px" aria-label="Pagination">
                  <button
                    onClick={goToPreviousPage}
                    disabled={currentPage === 1}
                    className="relative inline-flex items-center px-2 py-2 rounded-l-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    <span className="sr-only">Anterior</span>
                    <ChevronLeft className="h-5 w-5" />
                  </button>
                  
                  {/* Números das páginas */}
                  {Array.from({ length: Math.min(5, totalPages) }, (_, i) => {
                    let pageNumber: number;
                    if (totalPages <= 5) {
                      pageNumber = i + 1;
                    } else if (currentPage <= 3) {
                      pageNumber = i + 1;
                    } else if (currentPage >= totalPages - 2) {
                      pageNumber = totalPages - 4 + i;
                    } else {
                      pageNumber = currentPage - 2 + i;
                    }

                    return (
                      <button
                        key={pageNumber}
                        onClick={() => goToPage(pageNumber)}
                        className={`relative inline-flex items-center px-4 py-2 border text-sm font-medium ${
                          currentPage === pageNumber
                            ? 'z-10 bg-primary-50 border-primary-500 text-primary-600'
                            : 'bg-white border-gray-300 text-gray-500 hover:bg-gray-50'
                        }`}
                      >
                        {pageNumber}
                      </button>
                    );
                  })}
                  
                  <button
                    onClick={goToNextPage}
                    disabled={currentPage === totalPages}
                    className="relative inline-flex items-center px-2 py-2 rounded-r-md border border-gray-300 bg-white text-sm font-medium text-gray-500 hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed"
                  >
                    <span className="sr-only">Próxima</span>
                    <ChevronRight className="h-5 w-5" />
                  </button>
                </nav>
              </div>
            </div>
          </div>
        )}
      </div>

      {/* Modal de Nova Proposta */}
      <Modal
        isOpen={isModalOpen}
        onClose={() => setIsModalOpen(false)}
        title="Nova Proposta"
      >
        <ProposalForm
          onSuccess={(proposal) => {
            setIsModalOpen(false);
            loadProposals();
            toast.success('Proposta criada com sucesso!');
          }}
          onCancel={() => setIsModalOpen(false)}
        />
      </Modal>

      {/* Modal de Detalhes da Proposta */}
      {isDetailsModalOpen && selectedProposal && (
        <ProposalDetails
          proposal={selectedProposal}
          onClose={() => {
            setIsDetailsModalOpen(false);
            setSelectedProposal(null);
          }}
          onStatusUpdated={loadProposals}
        />
      )}

      {/* Modal de Edição da Proposta */}
      <Modal
        isOpen={isEditModalOpen}
        onClose={() => {
          setIsEditModalOpen(false);
          setSelectedProposal(null);
        }}
        title="Editar Proposta"
      >
        {selectedProposal && (
          <ProposalForm
            proposal={selectedProposal}
            onSuccess={(proposal) => {
              setIsEditModalOpen(false);
              setSelectedProposal(null);
              loadProposals();
              toast.success('Proposta atualizada com sucesso!');
            }}
            onCancel={() => {
              setIsEditModalOpen(false);
              setSelectedProposal(null);
            }}
          />
        )}
      </Modal>
    </div>
  );
};

export default ProposalList; 