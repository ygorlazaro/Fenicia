import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import {
  CAvatar,
  CDropdown,
  CDropdownDivider,
  CDropdownItem,
  CDropdownMenu,
  CDropdownToggle,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilUser, cilAccountLogout } from '@coreui/icons';
import { clearAuth, getCompanyId, getToken } from '../../services/client';

const AppHeaderDropdown = () => {
  const navigate = useNavigate();
  const [userName, setUserName] = useState('');
  const [companyName, setCompanyName] = useState('');

  useEffect(() => {
    // Get user info from token
    const token = getToken();
    if (token) {
      try {
        const tokenPayload = JSON.parse(atob(token.split('.')[1]));
        setUserName(tokenPayload.name || tokenPayload.email || 'Usuário');
      } catch (err) {
        console.error('Failed to parse token:', err);
        setUserName('Usuário');
      }
    }

    // Get company name from localStorage
    const companyId = getCompanyId();
    const companyNameStored = localStorage.getItem('company_name');
    if (companyId && companyNameStored) {
      setCompanyName(companyNameStored);
    } else if (companyId) {
      setCompanyName('Empresa');
    }
  }, []);

  const handleLogout = () => {
    clearAuth();
    navigate('/auth/login');
  };

  return (
    <CDropdown variant="nav-item">
      <CDropdownToggle placement="bottom-end" className="py-0 pe-0" caret={false}>
        <CAvatar color="primary" textColor="white" size="md">
          {userName.charAt(0).toUpperCase()}
        </CAvatar>
      </CDropdownToggle>
      <CDropdownMenu className="pt-0" placement="bottom-end">
        <div className="p-3">
          <div className="fw-semibold">{userName}</div>
          <small className="text-muted">{companyName}</small>
        </div>
        <CDropdownDivider />
        <CDropdownItem onClick={handleLogout}>
          <CIcon icon={cilAccountLogout} className="me-2" />
          Sair
        </CDropdownItem>
      </CDropdownMenu>
    </CDropdown>
  );
};

export default AppHeaderDropdown;
