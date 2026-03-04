import React from 'react';
import { useTranslation } from 'react-i18next';
import CIcon from '@coreui/icons-react';
import {
    cilCalculator,
    cilPeople,
    cilCart,
    cilBuilding,
    cilLayers,
    cilMove,
    cilList,
    cilNotes,
    cilPaperclip,
    cilDiamond,
    cilUser
} from '@coreui/icons';
import { CNavGroup, CNavItem, CNavTitle } from '@coreui/react';

const createNav = (t) => [
  {
    component: CNavTitle,
    name: t('menu.modules') || 'Módulos',
  },
  {
    component: CNavItem,
    name: t('menu.subscription'),
    to: '/subscription',
    icon: <CIcon icon={cilDiamond} customClassName="nav-icon" />,
    badge: {
        color: 'success',
        text: 'NEW',
    },
  },
  {
    component: CNavGroup,
    name: t('menu.basic'),
    to: '/basic',
    icon: <CIcon icon={cilCalculator} customClassName="nav-icon" />,
    items: [
      {
        component: CNavItem,
        name: t('menu.positions'),
        to: '/basic/positions',
        icon: <CIcon icon={cilPeople} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.employees'),
        to: '/basic/employees',
        icon: <CIcon icon={cilPeople} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.customers'),
        to: '/basic/customers',
        icon: <CIcon icon={cilPeople} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.suppliers'),
        to: '/basic/suppliers',
        icon: <CIcon icon={cilBuilding} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.categories'),
        to: '/basic/product-categories',
        icon: <CIcon icon={cilLayers} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.products'),
        to: '/basic/products',
        icon: <CIcon customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.inventory'),
        to: '/basic/inventory',
        icon: <CIcon customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.stockMovements'),
        to: '/basic/stock-movements',
        icon: <CIcon icon={cilMove} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.orders'),
        to: '/basic/orders',
        icon: <CIcon icon={cilCart} customClassName="nav-icon" />,
      },
    ],
  },
  {
    component: CNavGroup,
    name: t('menu.project'),
    to: '/project',
    icon: <CIcon icon={cilList} customClassName="nav-icon" />,
    items: [
      {
        component: CNavItem,
        name: t('menu.projects'),
        to: '/project/projects',
        icon: <CIcon icon={cilList} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.status'),
        to: '/project/status',
        icon: <CIcon icon={cilLayers} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.tasks'),
        to: '/project/tasks',
        icon: <CIcon icon={cilNotes} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.subtasks'),
        to: '/project/subtasks',
        icon: <CIcon icon={cilNotes} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.comments'),
        to: '/project/comments',
        icon: <CIcon icon={cilNotes} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.attachments'),
        to: '/project/attachments',
        icon: <CIcon icon={cilPaperclip} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: t('menu.taskAssignees'),
        to: '/project/task-assignees',
        icon: <CIcon icon={cilUser} customClassName="nav-icon" />,
      },
    ],
  },
];

export const useNav = () => {
  const { t } = useTranslation();
  return createNav(t);
};

const _nav = [
  {
    component: CNavTitle,
    name: 'Módulos',
  },
  {
    component: CNavItem,
    name: 'Assinar Módulos',
    to: '/subscription',
    icon: <CIcon icon={cilDiamond} customClassName="nav-icon" />,
    badge: {
        color: 'success',
        text: 'NEW',
    },
  },
  {
    component: CNavGroup,
    name: 'Básico',
    to: '/basic',
    icon: <CIcon icon={cilCalculator} customClassName="nav-icon" />,
    items: [
      {
        component: CNavItem,
        name: 'Cargos',
        to: '/basic/positions',
        icon: <CIcon icon={cilPeople} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Funcionários',
        to: '/basic/employees',
        icon: <CIcon icon={cilPeople} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Clientes',
        to: '/basic/customers',
        icon: <CIcon icon={cilPeople} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Fornecedores',
        to: '/basic/suppliers',
        icon: <CIcon icon={cilBuilding} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Categorias',
        to: '/basic/product-categories',
        icon: <CIcon icon={cilLayers} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Produtos',
        to: '/basic/products',
        icon: <CIcon customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Estoque',
        to: '/basic/inventory',
        icon: <CIcon customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Movimentações',
        to: '/basic/stock-movements',
        icon: <CIcon icon={cilMove} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Pedidos',
        to: '/basic/orders',
        icon: <CIcon icon={cilCart} customClassName="nav-icon" />,
      },
    ],
  },
  {
    component: CNavGroup,
    name: 'Projetos',
    to: '/project',
    icon: <CIcon icon={cilList} customClassName="nav-icon" />,
    items: [
      {
        component: CNavItem,
        name: 'Projetos',
        to: '/project/projects',
        icon: <CIcon icon={cilList} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Status',
        to: '/project/status',
        icon: <CIcon icon={cilLayers} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Tarefas',
        to: '/project/tasks',
        icon: <CIcon icon={cilNotes} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Subtarefas',
        to: '/project/subtasks',
        icon: <CIcon icon={cilNotes} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Comentários',
        to: '/project/comments',
        icon: <CIcon icon={cilNotes} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Anexos',
        to: '/project/attachments',
        icon: <CIcon icon={cilPaperclip} customClassName="nav-icon" />,
      },
      {
        component: CNavItem,
        name: 'Responsáveis',
        to: '/project/task-assignees',
        icon: <CIcon icon={cilUser} customClassName="nav-icon" />,
      },
    ],
  },
];

export default _nav;
