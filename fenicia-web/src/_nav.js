import React from 'react';
import CIcon from '@coreui/icons-react';
import {
  cilCalculator,
  cilPeople,
  cilCart,
  cilBuilding,
  cilLayers,
  cilMove,
  cilStar,
} from '@coreui/icons';
import { CNavGroup, CNavItem, CNavTitle } from '@coreui/react';

const _nav = [
  {
    component: CNavTitle,
    name: 'Módulos',
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
    component: CNavItem,
    name: 'Assinar Módulos',
    to: '/subscription',
    icon: <CIcon icon={cilStar} customClassName="nav-icon" />,
    badge: {
      color: 'success',
      text: 'NEW',
    },
  },
];

export default _nav;
