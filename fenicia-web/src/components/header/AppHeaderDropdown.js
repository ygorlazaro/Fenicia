import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import {
  CAvatar,
  CDropdown,
  CDropdownDivider,
  CDropdownItem,
  CDropdownMenu,
  CDropdownToggle,
} from '@coreui/react';
import CIcon from '@coreui/icons-react';
import { cilUser, cilAccountLogout, cilBuilding } from '@coreui/icons';
import { clearAuth, getCompanyId, getToken } from '../../services/client';

const AppHeaderDropdown = ({ onCompanySelect }) => {
  const navigate = useNavigate();
  const { t } = useTranslation();
  const [userName, setUserName] = useState('');
  const [companyName, setCompanyName] = useState('');

  useEffect(() => {
    // Get user info from token
    const token = getToken();
    if (token) {
      try {
        const tokenPayload = JSON.parse(atob(token.split('.')[1]));
        setUserName(tokenPayload.name || tokenPayload.email || t('auth.welcome'));
      } catch (err) {
        console.error('Failed to parse token:', err);
        setUserName(t('auth.welcome'));
      }
    }

    // Get company name from localStorage
    const companyId = getCompanyId();
    const companyNameStored = localStorage.getItem('company_name');
    if (companyId && companyNameStored) {
      setCompanyName(companyNameStored);
    } else if (companyId) {
      setCompanyName(t('auth.selectCompany'));
    }
  }, [t]);

  const handleLogout = () => {
    clearAuth();
    navigate('/auth/login');
  };

  const handleProfile = () => {
    navigate('/profile');
  };

  const handleCompanySelect = () => {
    if (onCompanySelect) {
      onCompanySelect();
    }
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
          <small 
            className="text-muted" 
            style={{ cursor: 'pointer', textDecoration: 'underline' }}
            onClick={handleCompanySelect}
            title={t('auth.selectCompany')}
          >
            {companyName}
          </small>
        </div>
        <CDropdownDivider />
        <CDropdownItem onClick={handleProfile}>
          <CIcon icon={cilUser} className="me-2" />
          {t('menu.profile')}
        </CDropdownItem>
        <CDropdownItem onClick={handleCompanySelect}>
          <CIcon icon={cilBuilding} className="me-2" />
          {t('auth.selectCompany')}
        </CDropdownItem>
        <CDropdownItem onClick={handleLogout}>
          <CIcon icon={cilAccountLogout} className="me-2" />
          {t('auth.logout')}
        </CDropdownItem>
      </CDropdownMenu>
    </CDropdown>
  );
};

export default AppHeaderDropdown;
