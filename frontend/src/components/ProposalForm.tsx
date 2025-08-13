import React, { useState, useEffect } from 'react';
import { useForm } from 'react-hook-form';
import { motion } from 'framer-motion';
import { Plus, Loader2, Edit } from 'lucide-react';
import { proposalService } from '../services/api';
import { CreateProposalRequest, Proposal, ProposalStatus } from '../types';
import toast from 'react-hot-toast';



interface ProposalFormProps {
  proposal?: Proposal;
  onSuccess?: (proposal: any) => void;
  onCancel?: () => void;
}

const ProposalForm: React.FC<ProposalFormProps> = ({ proposal, onSuccess, onCancel }) => {
  const [loading, setLoading] = useState(false);
  const { register, handleSubmit, formState: { errors }, reset, setValue, watch } = useForm<CreateProposalRequest>();
  const isEditing = !!proposal;

  // Preencher formulário com dados da proposta se estiver editando
  useEffect(() => {
    if (proposal) {
      setValue('customerName', proposal.customerName);
      setValue('customerEmail', proposal.customerEmail);
      setValue('insuranceType', proposal.insuranceType);
      setValue('coverageAmount', proposal.coverageAmount);
      setValue('premiumAmount', proposal.premiumAmount);
    }
  }, [proposal, setValue]);

  // Debug: log dos valores quando proposal muda
  useEffect(() => {
    if (proposal) {
      console.log('Proposal data for form:', {
        customerName: proposal.customerName,
        customerEmail: proposal.customerEmail,
        insuranceType: proposal.insuranceType,
        coverageAmount: proposal.coverageAmount,
        premiumAmount: proposal.premiumAmount
      });
    }
  }, [proposal]);

  const insuranceTypes = [
    { value: 'Auto Insurance', label: 'Seguro Auto' },
    { value: 'Home Insurance', label: 'Seguro Residencial' },
    { value: 'Life Insurance', label: 'Seguro de Vida' },
    { value: 'Health Insurance', label: 'Seguro Saúde' },
    { value: 'Travel Insurance', label: 'Seguro Viagem' }
  ];

  const onSubmit = async (data: CreateProposalRequest) => {
    setLoading(true);
    try {
      if (isEditing && proposal) {
        const updatedProposal = await proposalService.updateProposal(proposal.id, data);
        toast.success('Proposta atualizada com sucesso!');
        onSuccess?.(updatedProposal);
      } else {
        const newProposal = await proposalService.createProposal(data);
        toast.success('Proposta criada com sucesso!');
        reset();
        onSuccess?.(newProposal);
      }
    } catch (error) {
      console.error('Error saving proposal:', error);
      toast.error(isEditing ? 'Erro ao atualizar proposta. Tente novamente.' : 'Erro ao criar proposta. Tente novamente.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="space-y-6">

      <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Nome do Cliente */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Nome do Cliente *
            </label>
            <input
              type="text"
              {...register('customerName', { 
                required: 'Nome é obrigatório',
                minLength: { value: 2, message: 'Nome deve ter pelo menos 2 caracteres' }
              })}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors.customerName ? 'border-red-500' : 'border-gray-300'
              }`}
              placeholder="Digite o nome completo"
            />
            {errors.customerName && (
              <p className="mt-1 text-sm text-red-600">{errors.customerName.message}</p>
            )}
          </div>

          {/* Email do Cliente */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Email *
            </label>
            <input
              type="email"
              {...register('customerEmail', { 
                required: 'Email é obrigatório',
                pattern: {
                  value: /^[A-Z0-9._%+-]+@[A-Z0-9.-]+\.[A-Z]{2,}$/i,
                  message: 'Email inválido'
                }
              })}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors.customerEmail ? 'border-red-500' : 'border-gray-300'
              }`}
              placeholder="cliente@email.com"
            />
            {errors.customerEmail && (
              <p className="mt-1 text-sm text-red-600">{errors.customerEmail.message}</p>
            )}
          </div>
        </div>

        {/* Tipo de Seguro */}
        <div>
          <label className="block text-sm font-medium text-gray-700 mb-2">
            Tipo de Seguro *
          </label>
          <select
            {...register('insuranceType', { required: 'Tipo de seguro é obrigatório' })}
            defaultValue={proposal?.insuranceType || ''}
            className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
              errors.insuranceType ? 'border-red-500' : 'border-gray-300'
            }`}
          >
            <option value="">Selecione o tipo de seguro</option>
            {insuranceTypes.map((type) => (
              <option key={type.value} value={type.value}>
                {type.label}
              </option>
            ))}
          </select>
          {errors.insuranceType && (
            <p className="mt-1 text-sm text-red-600">{errors.insuranceType.message}</p>
          )}
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
          {/* Valor de Cobertura */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Valor de Cobertura (R$) *
            </label>
            <input
              type="number"
              step="0.01"
              min="1000"
              {...register('coverageAmount', { 
                required: 'Valor de cobertura é obrigatório',
                min: { value: 1000, message: 'Valor mínimo é R$ 1.000,00' },
                max: { value: 10000000, message: 'Valor máximo é R$ 10.000.000,00' }
              })}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors.coverageAmount ? 'border-red-500' : 'border-gray-300'
              }`}
              placeholder="50000"
            />
            {errors.coverageAmount && (
              <p className="mt-1 text-sm text-red-600">{errors.coverageAmount.message}</p>
            )}
          </div>

          {/* Valor do Prêmio */}
          <div>
            <label className="block text-sm font-medium text-gray-700 mb-2">
              Valor do Prêmio (R$) *
            </label>
            <input
              type="number"
              step="0.01"
              min="100"
              {...register('premiumAmount', { 
                required: 'Valor do prêmio é obrigatório',
                min: { value: 100, message: 'Valor mínimo é R$ 100,00' },
                max: { value: 1000000, message: 'Valor máximo é R$ 1.000.000,00' }
              })}
              className={`w-full px-3 py-2 border rounded-md focus:outline-none focus:ring-2 focus:ring-primary-500 ${
                errors.premiumAmount ? 'border-red-500' : 'border-gray-300'
              }`}
              placeholder="1200"
            />
            {errors.premiumAmount && (
              <p className="mt-1 text-sm text-red-600">{errors.premiumAmount.message}</p>
            )}
          </div>
        </div>

        {/* Botões */}
        <div className="flex justify-end space-x-4 pt-6 border-t">
          <button
            type="button"
            onClick={onCancel}
            className="px-6 py-2 border border-gray-300 rounded-md text-gray-700 hover:bg-gray-50 focus:outline-none focus:ring-2 focus:ring-gray-500"
          >
            Cancelar
          </button>
          <button
            type="submit"
            disabled={loading}
            className="px-6 py-2 bg-primary-600 text-white rounded-md hover:bg-primary-700 focus:outline-none focus:ring-2 focus:ring-primary-500 disabled:opacity-50 disabled:cursor-not-allowed flex items-center space-x-2"
          >
            {loading ? (
              <>
                <Loader2 className="h-4 w-4 animate-spin" />
                <span>{isEditing ? 'Atualizando...' : 'Criando...'}</span>
              </>
            ) : (
              <>
                {isEditing ? <Edit className="h-4 w-4" /> : <Plus className="h-4 w-4" />}
                <span>{isEditing ? 'Atualizar Proposta' : 'Criar Proposta'}</span>
              </>
            )}
          </button>
        </div>
      </form>
    </div>
  );
};

export default ProposalForm; 