import React, { useState, useEffect } from 'react';
import { motion } from 'framer-motion';
import { 
  TrendingUp, 
  FileText, 
  Clock, 
  CheckCircle, 
  XCircle, 
  DollarSign,
  Users,
  Activity
} from 'lucide-react';
import { BarChart, Bar, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer } from 'recharts';
import { dashboardService } from '../services/api';
import { DashboardStats } from '../types';

const Dashboard: React.FC = () => {
  const [stats, setStats] = useState<DashboardStats>({
    totalProposals: 0,
    pendingProposals: 0,
    approvedProposals: 0,
    rejectedProposals: 0,
    totalContracts: 0,
    totalRevenue: 0
  });
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadStats();
    const interval = setInterval(loadStats, 30000); // Atualizar a cada 30 segundos
    return () => clearInterval(interval);
  }, []);

  const loadStats = async () => {
    try {
      const data = await dashboardService.getStats();
      setStats(data);
    } catch (error) {
      console.error('Error loading dashboard stats:', error);
    } finally {
      setLoading(false);
    }
  };

  const StatCard: React.FC<{
    title: string;
    value: number | string;
    icon: React.ReactNode;
    color: string;
    change?: number;
  }> = ({ title, value, icon, color, change }) => (
    <motion.div
      initial={{ opacity: 0, y: 20 }}
      animate={{ opacity: 1, y: 0 }}
      transition={{ duration: 0.5 }}
      className={`bg-white rounded-lg shadow-md p-6 border-l-4 ${color}`}
    >
      <div className="flex items-center justify-between">
        <div>
          <p className="text-sm font-medium text-gray-600">{title}</p>
          <p className="text-2xl font-bold text-gray-900">
            {typeof value === 'number' && value >= 1000 
              ? `R$ ${(value / 1000).toFixed(1)}k` 
              : typeof value === 'number' 
                ? value.toLocaleString('pt-BR') 
                : value}
          </p>
          {change !== undefined && (
            <p className={`text-sm ${change >= 0 ? 'text-green-600' : 'text-red-600'}`}>
              {change >= 0 ? '+' : ''}{change}% desde o último mês
            </p>
          )}
        </div>
        <div className="p-3 bg-gray-50 rounded-full">
          {icon}
        </div>
      </div>
    </motion.div>
  );

  if (loading) {
    return (
      <div className="flex items-center justify-center h-64">
        <div className="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
      </div>
    );
  }

  return (
    <div className="space-y-6">
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Dashboard</h1>
        <p className="text-gray-600">Visão geral do sistema de seguros</p>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
        <StatCard
          title="Total de Propostas"
          value={stats.totalProposals}
          icon={<FileText className="h-6 w-6 text-blue-600" />}
          color="border-blue-500"
          change={12}
        />
        
        <StatCard
          title="Em Análise"
          value={stats.pendingProposals}
          icon={<Clock className="h-6 w-6 text-yellow-600" />}
          color="border-yellow-500"
          change={-5}
        />
        
        <StatCard
          title="Aprovadas"
          value={stats.approvedProposals}
          icon={<CheckCircle className="h-6 w-6 text-green-600" />}
          color="border-green-500"
          change={8}
        />
        
        <StatCard
          title="Rejeitadas"
          value={stats.rejectedProposals}
          icon={<XCircle className="h-6 w-6 text-red-600" />}
          color="border-red-500"
          change={-2}
        />
      </div>

      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <motion.div
          initial={{ opacity: 0, x: -20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.5, delay: 0.2 }}
          className="bg-white rounded-lg shadow-md p-6"
        >
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold text-gray-900">Contratos</h3>
            <Users className="h-5 w-5 text-gray-400" />
          </div>
          <div className="text-3xl font-bold text-gray-900 mb-2">
            {stats.totalContracts}
          </div>
          <p className="text-sm text-gray-600">
            Contratos ativos no sistema
          </p>
        </motion.div>

        <motion.div
          initial={{ opacity: 0, x: 20 }}
          animate={{ opacity: 1, x: 0 }}
          transition={{ duration: 0.5, delay: 0.3 }}
          className="bg-white rounded-lg shadow-md p-6"
        >
          <div className="flex items-center justify-between mb-4">
            <h3 className="text-lg font-semibold text-gray-900">Receita Total</h3>
            <DollarSign className="h-5 w-5 text-gray-400" />
          </div>
          <div className="text-3xl font-bold text-gray-900 mb-2">
            R$ {stats.totalRevenue.toLocaleString('pt-BR')}
          </div>
          <p className="text-sm text-gray-600">
            Valor total dos contratos
          </p>
        </motion.div>
      </div>

      <motion.div
        initial={{ opacity: 0, y: 20 }}
        animate={{ opacity: 1, y: 0 }}
        transition={{ duration: 0.5, delay: 0.4 }}
        className="bg-white rounded-lg shadow-md p-6"
      >
        <div className="flex items-center justify-between mb-6">
          <h3 className="text-lg font-semibold text-gray-900">Status das Propostas</h3>
          <Activity className="h-5 w-5 text-gray-400" />
        </div>
        
        <div className="h-64">
          <ResponsiveContainer width="100%" height="100%">
            <BarChart
              data={[
                {
                  name: 'Em Análise',
                  'Em Análise': stats.pendingProposals,
                  'Aprovadas': 0,
                  'Rejeitadas': 0
                },
                {
                  name: 'Aprovadas',
                  'Em Análise': 0,
                  'Aprovadas': stats.approvedProposals,
                  'Rejeitadas': 0
                },
                {
                  name: 'Rejeitadas',
                  'Em Análise': 0,
                  'Aprovadas': 0,
                  'Rejeitadas': stats.rejectedProposals
                }
              ]}
              margin={{
                top: 20,
                right: 30,
                left: 20,
                bottom: 5,
              }}
            >
              <CartesianGrid strokeDasharray="3 3" stroke="#f3f4f6" />
              <XAxis 
                dataKey="name" 
                axisLine={false}
                tickLine={false}
                tick={{ fontSize: 12, fill: '#6b7280' }}
              />
              <YAxis 
                axisLine={false}
                tickLine={false}
                tick={{ fontSize: 12, fill: '#6b7280' }}
              />
              <Tooltip 
                contentStyle={{
                  backgroundColor: '#ffffff',
                  border: '1px solid #e5e7eb',
                  borderRadius: '8px',
                  boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)'
                }}
                formatter={(value: any) => [value, 'Quantidade']}
                labelFormatter={(label) => `Status: ${label}`}
              />
              <Bar 
                dataKey="Em Análise" 
                radius={[4, 4, 0, 0]}
                fill="#f59e0b"
              />
              <Bar 
                dataKey="Aprovadas" 
                radius={[4, 4, 0, 0]}
                fill="#10b981"
              />
              <Bar 
                dataKey="Rejeitadas" 
                radius={[4, 4, 0, 0]}
                fill="#ef4444"
              />
            </BarChart>
          </ResponsiveContainer>
        </div>
        
        <div className="mt-4 grid grid-cols-3 gap-4 text-center">
          <div className="flex flex-col items-center">
            <div className="w-3 h-3 bg-yellow-500 rounded-full mb-1"></div>
            <span className="text-xs text-gray-600">Em Análise</span>
            <span className="text-sm font-semibold text-gray-900">{stats.pendingProposals}</span>
          </div>
          <div className="flex flex-col items-center">
            <div className="w-3 h-3 bg-green-500 rounded-full mb-1"></div>
            <span className="text-xs text-gray-600">Aprovadas</span>
            <span className="text-sm font-semibold text-gray-900">{stats.approvedProposals}</span>
          </div>
          <div className="flex flex-col items-center">
            <div className="w-3 h-3 bg-red-500 rounded-full mb-1"></div>
            <span className="text-xs text-gray-600">Rejeitadas</span>
            <span className="text-sm font-semibold text-gray-900">{stats.rejectedProposals}</span>
          </div>
        </div>
      </motion.div>
    </div>
  );
};

export default Dashboard; 