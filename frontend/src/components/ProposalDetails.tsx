import React, { useState, useEffect } from 'react';
import { X, CheckCircle, XCircle, Clock, FileText, Check, X as XIcon, FileCheck } from 'lucide-react';
import { Proposal, ProposalStatus, UpdateProposalStatusRequest, Contract } from '../types';
import { proposalService, contractService } from '../services/api';
import { toast } from 'react-hot-toast';

interface ProposalDetailsProps {
  proposal: Proposal | null;
  onClose: () => void;
  onStatusUpdated?: () => void;
}

const ProposalDetails: React.FC<ProposalDetailsProps> = ({ proposal, onClose, onStatusUpdated }) => {
  const [loading, setLoading] = useState(false);
  const [rejectionReason, setRejectionReason] = useState('');
  const [showRejectionForm, setShowRejectionForm] = useState(false);
  const [existingContract, setExistingContract] = useState<Contract | null>(null);
  const [contractLoading, setContractLoading] = useState(false);

  // Verificar se já existe um contrato para esta proposta
  useEffect(() => {
    const checkExistingContract = async () => {
      if (proposal && proposal.status === ProposalStatus.Approved) {
        try {
          const contract = await contractService.getContractByProposalId(proposal.id);
          setExistingContract(contract);
        } catch (error) {
          // Se der 404, significa que não existe contrato
          if (error && typeof error === 'object' && 'response' in error && (error.response as any)?.status === 404) {
            setExistingContract(null);
          } else {
            console.error('Erro ao verificar contrato existente:', error);
          }
        }
      } else {
        setExistingContract(null);
      }
    };

    checkExistingContract();
  }, [proposal]);

  if (!proposal) return null;

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

  const handleApprove = async () => {
    if (proposal.status !== ProposalStatus.UnderReview) {
      toast.error('Apenas propostas em análise podem ser aprovadas');
      return;
    }

    setLoading(true);
    try {
      const updateData: UpdateProposalStatusRequest = {
        status: ProposalStatus.Approved
      };

      await proposalService.updateProposalStatus(proposal.id, updateData);
      toast.success('Proposta aprovada com sucesso!');
      onStatusUpdated?.();
      onClose();
    } catch (error) {
      console.error('Erro ao aprovar proposta:', error);
      toast.error('Erro ao aprovar proposta');
    } finally {
      setLoading(false);
    }
  };

  const handleReject = async () => {
    if (proposal.status !== ProposalStatus.UnderReview) {
      toast.error('Apenas propostas em análise podem ser rejeitadas');
      return;
    }

    if (!rejectionReason.trim()) {
      toast.error('Motivo da rejeição é obrigatório');
      return;
    }

    setLoading(true);
    try {
      const updateData: UpdateProposalStatusRequest = {
        status: ProposalStatus.Rejected,
        rejectionReason: rejectionReason.trim()
      };

      await proposalService.updateProposalStatus(proposal.id, updateData);
      toast.success('Proposta rejeitada com sucesso!');
      onStatusUpdated?.();
      onClose();
    } catch (error) {
      console.error('Erro ao rejeitar proposta:', error);
      toast.error('Erro ao rejeitar proposta');
    } finally {
      setLoading(false);
    }
  };

  const canUpdateStatus = proposal.status === ProposalStatus.UnderReview;

  const handleCreateContract = async () => {
    if (proposal.status !== ProposalStatus.Approved) {
      toast.error('Apenas propostas aprovadas podem ter contratos criados');
      return;
    }

    if (existingContract) {
      toast.error('Já existe um contrato para esta proposta');
      return;
    }

    setContractLoading(true);
    try {
      const newContract = await contractService.createContract({ proposalId: proposal.id });
      setExistingContract(newContract);
      toast.success('Contrato criado com sucesso!');
      onStatusUpdated?.();
      // Fechar a modal após criar o contrato com sucesso
      onClose();
    } catch (error) {
      console.error('Erro ao criar contrato:', error);
      toast.error('Erro ao criar contrato');
    } finally {
      setContractLoading(false);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl max-w-2xl w-full mx-4 max-h-[90vh] overflow-y-auto">
        <div className="flex justify-between items-center p-6 border-b">
          <h2 className="text-xl font-semibold text-gray-900">Detalhes da Proposta</h2>
          <button
            onClick={onClose}
            className="text-gray-400 hover:text-gray-600"
          >
            <X className="h-6 w-6" />
          </button>
        </div>

        <div className="p-6 space-y-6">
          {/* Status */}
          <div className="flex items-center space-x-3">
            {getStatusIcon(proposal.status)}
            <span className={`inline-flex px-3 py-1 text-sm font-semibold rounded-full ${getStatusColor(proposal.status)}`}>
              {getStatusLabel(proposal.status)}
            </span>
          </div>

          {/* Informações do Cliente */}
          <div>
            <h3 className="text-lg font-medium text-gray-900 mb-3">Informações do Cliente</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Nome</label>
                <p className="mt-1 text-sm text-gray-900">{proposal.customerName}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Email</label>
                <p className="mt-1 text-sm text-gray-900">{proposal.customerEmail}</p>
              </div>
            </div>
          </div>

          {/* Informações do Seguro */}
          <div>
            <h3 className="text-lg font-medium text-gray-900 mb-3">Informações do Seguro</h3>
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Tipo de Seguro</label>
                <p className="mt-1 text-sm text-gray-900">{proposal.insuranceType}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Valor da Cobertura</label>
                <p className="mt-1 text-sm text-gray-900">R$ {proposal.coverageAmount.toLocaleString('pt-BR')}</p>
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700">Valor do Prêmio</label>
                <p className="mt-1 text-sm text-gray-900">R$ {proposal.premiumAmount.toLocaleString('pt-BR')}</p>
              </div>
            </div>
          </div>

          {/* Datas */}
          <div>
            <h3 className="text-lg font-medium text-gray-900 mb-3">Datas</h3>
            <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700">Data de Criação</label>
                <p className="mt-1 text-sm text-gray-900">
                  {new Date(proposal.createdAt).toLocaleDateString('pt-BR')} às {new Date(proposal.createdAt).toLocaleTimeString('pt-BR')}
                </p>
              </div>
              {proposal.updatedAt && (
                <div>
                  <label className="block text-sm font-medium text-gray-700">Última Atualização</label>
                  <p className="mt-1 text-sm text-gray-900">
                    {new Date(proposal.updatedAt).toLocaleDateString('pt-BR')} às {new Date(proposal.updatedAt).toLocaleTimeString('pt-BR')}
                  </p>
                </div>
              )}
            </div>
          </div>

          {/* Motivo da Rejeição */}
          {proposal.status === ProposalStatus.Rejected && proposal.rejectionReason && (
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-3">Motivo da Rejeição</h3>
              <div className="bg-red-50 border border-red-200 rounded-md p-4">
                <p className="text-sm text-red-800">{proposal.rejectionReason}</p>
              </div>
            </div>
          )}

          {/* Contrato Existente */}
          {existingContract && (
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-3">Contrato Gerado</h3>
              <div className="bg-green-50 border border-green-200 rounded-md p-4">
                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Número do Contrato</label>
                    <p className="mt-1 text-sm text-gray-900 font-mono">{existingContract.contractNumber}</p>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Data do Contrato</label>
                    <p className="mt-1 text-sm text-gray-900">
                      {new Date(existingContract.contractDate).toLocaleDateString('pt-BR')}
                    </p>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700">Valor do Prêmio</label>
                    <p className="mt-1 text-sm text-gray-900">R$ {existingContract.premiumAmount.toLocaleString('pt-BR')}</p>
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-gray-700">ID do Contrato</label>
                    <p className="mt-1 text-sm text-green-700 font-mono font-semibold bg-green-100 px-2 py-1 rounded">
                      {existingContract.id}
                    </p>
                  </div>
                </div>
              </div>
            </div>
          )}

          {/* Informações do Contrato (quando não existe) */}
          {proposal.status === ProposalStatus.Approved && !existingContract && (
            <div>
              <h3 className="text-lg font-medium text-gray-900 mb-3">Contrato</h3>
              <div className="bg-gray-50 border border-gray-200 rounded-md p-4">
                <div className="text-center">
                  <FileText className="h-8 w-8 text-gray-400 mx-auto mb-2" />
                  <p className="text-sm text-gray-600 mb-2">Nenhum contrato foi gerado para esta proposta</p>
                  <p className="text-xs text-gray-500">Clique em "Criar Contrato" para gerar um novo contrato</p>
                </div>
              </div>
            </div>
          )}

          {/* ID da Proposta */}
          <div>
            <label className="block text-sm font-medium text-gray-700">ID da Proposta</label>
            <p className="mt-1 text-sm text-gray-500 font-mono">{proposal.id}</p>
          </div>
        </div>

        <div className="flex justify-between items-center p-6 border-t">
          <div className="flex space-x-2">
            {canUpdateStatus && (
              <>
                <button
                  onClick={handleApprove}
                  disabled={loading}
                  className="bg-green-600 text-white px-4 py-2 rounded-md hover:bg-green-700 focus:outline-none focus:ring-2 focus:ring-green-500 disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
                >
                  {loading ? (
                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                  ) : (
                    <Check className="h-4 w-4" />
                  )}
                  <span>Aprovar</span>
                </button>
                
                <button
                  onClick={() => setShowRejectionForm(true)}
                  disabled={loading}
                  className="bg-red-600 text-white px-4 py-2 rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
                >
                  <XIcon className="h-4 w-4" />
                  <span>Rejeitar</span>
                </button>
              </>
            )}

            {proposal.status === ProposalStatus.Approved && !existingContract && (
              <button
                onClick={handleCreateContract}
                disabled={contractLoading}
                className="bg-blue-600 text-white px-6 py-2 rounded-md hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-blue-500 disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2 shadow-lg"
              >
                {contractLoading ? (
                  <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                ) : (
                  <FileCheck className="h-4 w-4" />
                )}
                <span>Criar Contrato</span>
              </button>
            )}

            {proposal.status === ProposalStatus.Approved && existingContract && (
              <div className="flex items-center space-x-2 px-4 py-2 bg-green-50 border border-green-200 rounded-md">
                <FileCheck className="h-4 w-4 text-green-600" />
                <span className="text-sm text-green-800 font-medium">Contrato já criado</span>
                <span className="text-xs text-green-600">ID: {existingContract.id}</span>
              </div>
            )}
          </div>
          
          <button
            onClick={onClose}
            className="bg-gray-600 text-white px-4 py-2 rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500"
          >
            Fechar
          </button>
        </div>

        {/* Modal de Rejeição */}
        {showRejectionForm && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg shadow-xl max-w-md w-full mx-4">
              <div className="flex justify-between items-center p-6 border-b">
                <h3 className="text-lg font-semibold text-gray-900">Rejeitar Proposta</h3>
                <button
                  onClick={() => setShowRejectionForm(false)}
                  className="text-gray-400 hover:text-gray-600"
                >
                  <X className="h-6 w-6" />
                </button>
              </div>
              
              <div className="p-6">
                <label className="block text-sm font-medium text-gray-700 mb-2">
                  Motivo da Rejeição *
                </label>
                <textarea
                  value={rejectionReason}
                  onChange={(e) => setRejectionReason(e.target.value)}
                  className="w-full px-3 py-2 border border-gray-300 rounded-md focus:outline-none focus:ring-2 focus:ring-red-500"
                  rows={4}
                  placeholder="Digite o motivo da rejeição..."
                  disabled={loading}
                />
              </div>
              
              <div className="flex justify-end space-x-2 p-6 border-t">
                <button
                  onClick={() => setShowRejectionForm(false)}
                  disabled={loading}
                  className="bg-gray-600 text-white px-4 py-2 rounded-md hover:bg-gray-700 focus:outline-none focus:ring-2 focus:ring-gray-500 disabled:opacity-50"
                >
                  Cancelar
                </button>
                <button
                  onClick={handleReject}
                  disabled={loading || !rejectionReason.trim()}
                  className="bg-red-600 text-white px-4 py-2 rounded-md hover:bg-red-700 focus:outline-none focus:ring-2 focus:ring-red-500 disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
                >
                  {loading ? (
                    <div className="animate-spin rounded-full h-4 w-4 border-b-2 border-white"></div>
                  ) : (
                    <XIcon className="h-4 w-4" />
                  )}
                  <span>Confirmar Rejeição</span>
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ProposalDetails; 