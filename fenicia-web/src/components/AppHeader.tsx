import React, { useEffect, useRef, useState } from 'react'
import { NavLink } from 'react-router-dom'
import { useSelector, useDispatch } from 'react-redux'
import {
    CContainer,
    CHeader,
    CHeaderNav,
    CHeaderToggler,
    CNavLink,
    CNavItem, useColorModes,
} from '@coreui/react'
import CIcon from '@coreui/icons-react'
import {cilContrast, cilMenu, cilMoon, cilSun} from '@coreui/icons'
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
    { path: '/profile', label: t('menu.profile') }
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
