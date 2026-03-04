import React, { useEffect, useRef, useState } from 'react'
import { NavLink } from 'react-router-dom'
import { useSelector, useDispatch } from 'react-redux'
import {
  CContainer,
  CDropdown,
  CDropdownItem,
  CDropdownMenu,
  CDropdownToggle,
  CHeader,
  CHeaderNav,
  CHeaderToggler,
  CNavLink,
  CNavItem,
  useColorModes,
} from '@coreui/react'
import CIcon from '@coreui/icons-react'
import {
  cilBell,
  cilContrast,
  cilEnvelopeOpen,
  cilList,
  cilMenu,
  cilMoon,
  cilSun,
} from '@coreui/icons'
import { useTranslation } from 'react-i18next'

import { AppBreadcrumb } from './index'
import { AppHeaderDropdown } from './header/index'
import LanguageSelector from './LanguageSelector'
import CompanySelectModal from './CompanySelectModal'
import AuthCompanyClient from '../services/auth-company-client'
import { setCompanyId } from '../services/client'

const companyClient = new AuthCompanyClient("http://localhost:5144")

const AppHeader = () => {
  const headerRef = useRef()
  const { t } = useTranslation()
  const { colorMode, setColorMode } = useColorModes('fenicia-gato-ninja-theme')

  const dispatch = useDispatch()
  const sidebarShow = useSelector((state) => state.sidebarShow)
  
  // Company selection modal state
  const [showCompanyModal, setShowCompanyModal] = useState(false)
  const [companies, setCompanies] = useState([])
  const [loadingCompanies, setLoadingCompanies] = useState(false)
  const [companiesError, setCompaniesError] = useState(null)

  useEffect(() => {
    document.addEventListener('scroll', () => {
      headerRef.current &&
        headerRef.current.classList.toggle('shadow-sm', document.documentElement.scrollTop > 0)
    })
  }, [])

  const handleOpenCompanySelect = async () => {
    setShowCompanyModal(true)
    setLoadingCompanies(true)
    setCompaniesError(null)
    
    try {
      const response = await companyClient.getCompaniesByUser(1, 50)
      const companiesList = Array.isArray(response) ? response : response.items || response.data || []
      setCompanies(companiesList)
      
      if (companiesList.length === 0) {
        setCompaniesError(t('auth.noCompanies'))
      }
    } catch (err) {
      console.error('Failed to load companies:', err)
      setCompaniesError(err.response?.data?.title || err.message || t('common.error'))
    } finally {
      setLoadingCompanies(false)
    }
  }

  const handleSelectCompany = (company) => {
    // Persist company ID and name to localStorage
    setCompanyId(company.id)
    localStorage.setItem('company_name', company.name)
    
    // Close modal
    setShowCompanyModal(false)
    
    // Reload page to apply new company context
    window.location.reload()
  }

  // Translate menu items
  const menuItems = [
    { path: '/dashboard', label: t('menu.dashboard') },
    { path: '/auth/user', label: t('menu.employees') },
    { path: '#', label: t('common.actions') }
  ]

  return (
    <CHeader position="sticky" className="mb-4 p-0" ref={headerRef}>
      <CContainer className="border-bottom px-4" fluid>
        <CHeaderToggler
          onClick={() => dispatch({ type: 'set', sidebarShow: !sidebarShow })}
          style={{ marginInlineStart: '-14px' }}
        >
          <CIcon icon={cilMenu} size="lg" />
        </CHeaderToggler>
        <CHeaderNav className="d-none d-md-flex">
          {menuItems.map((item, index) => (
            <CNavItem key={index}>
              <CNavLink to={item.path} as={NavLink}>
                {item.label}
              </CNavLink>
            </CNavItem>
          ))}
        </CHeaderNav>
        <CHeaderNav>
          <CDropdown variant="nav-item" placement="bottom-end">
            <CDropdownToggle caret={false}>
              {colorMode === 'dark' ? (
                <CIcon icon={cilMoon} size="lg" />
              ) : colorMode === 'auto' ? (
                <CIcon icon={cilContrast} size="lg" />
              ) : (
                <CIcon icon={cilSun} size="lg" />
              )}
            </CDropdownToggle>
            <CDropdownMenu>
              <CDropdownItem
                active={colorMode === 'light'}
                className="d-flex align-items-center"
                as="button"
                type="button"
                onClick={() => setColorMode('light')}
              >
                <CIcon className="me-2" icon={cilSun} size="lg" /> {t('common.light')}
              </CDropdownItem>
              <CDropdownItem
                active={colorMode === 'dark'}
                className="d-flex align-items-center"
                as="button"
                type="button"
                onClick={() => setColorMode('dark')}
              >
                <CIcon className="me-2" icon={cilMoon} size="lg" /> {t('common.dark')}
              </CDropdownItem>
              <CDropdownItem
                active={colorMode === 'auto'}
                className="d-flex align-items-center"
                as="button"
                type="button"
                onClick={() => setColorMode('auto')}
              >
                <CIcon className="me-2" icon={cilContrast} size="lg" /> {t('common.auto')}
              </CDropdownItem>
            </CDropdownMenu>
          </CDropdown>
          <li className="nav-item py-1">
            <div className="vr h-100 mx-2 text-body text-opacity-75"></div>
          </li>
          <LanguageSelector />
          <li className="nav-item py-1">
            <div className="vr h-100 mx-2 text-body text-opacity-75"></div>
          </li>
          <AppHeaderDropdown onCompanySelect={handleOpenCompanySelect} />
        </CHeaderNav>
      </CContainer>
      <CContainer className="px-4" fluid>
        <AppBreadcrumb />
      </CContainer>

      {/* Company Select Modal */}
      <CompanySelectModal
        visible={showCompanyModal}
        companies={companies}
        loading={loadingCompanies}
        error={companiesError}
        onSelect={handleSelectCompany}
      />
    </CHeader>
  )
}

export default AppHeader
