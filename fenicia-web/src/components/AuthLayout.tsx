import React from 'react'
import { CContainer, CRow, CCol } from '@coreui/react'
import feniciaLogo from 'src/assets/brand/fenicia.svg'
import LanguageSelector from './LanguageSelector'

const AuthLayout = ({ children }) => {
  return (
    <div className="bg-body-tertiary min-vh-100 d-flex flex-row align-items-center">
      <CContainer fluid>
        {/* Language Selector - Top Right Corner */}
        <div className="position-absolute top-0 end-0 p-3" style={{ zIndex: 1000 }}>
          <LanguageSelector />
        </div>
        
        <CRow className="justify-content-center align-items-center g-0">
          {/* Logo Section - Left on desktop, Top on mobile */}
          <CCol lg={6} xl={5} className="d-flex justify-content-center align-items-center p-4 auth-logo-section">
            <div className="text-center">
              <img
                src={feniciaLogo}
                alt="Fenicia - Gato Ninja Site"
                className="img-fluid auth-logo"
                style={{ 
                  maxWidth: '400px', 
                  width: '100%',
                  height: 'auto'
                }}
              />
              <h2 className="mt-4 text-primary fw-bold">Fenicia</h2>
              <p className="text-muted">Gato Ninja</p>
            </div>
          </CCol>
          
          {/* Form Section - Right on desktop, Bottom on mobile */}
          <CCol lg={6} xl={7} className="d-flex justify-content-center align-items-center p-4">
            <div className="w-100" style={{ maxWidth: '450px' }}>
              {children}
            </div>
          </CCol>
        </CRow>
      </CContainer>
      
      {/* Custom CSS for responsive layout */}
      <style>{`
        .auth-logo-section {
          min-height: 300px;
        }
        
        .auth-logo {
          filter: drop-shadow(0 4px 6px rgba(0, 0, 0, 0.1));
          transition: transform 0.3s ease;
        }
        
        .auth-logo:hover {
          transform: scale(1.02);
        }
        
        @media (max-width: 991.98px) {
          .auth-logo-section {
            min-height: auto;
            padding-bottom: 2rem !important;
          }
          
          .auth-logo {
            max-width: 250px !important;
          }
        }
        
        @media (max-width: 575.98px) {
          .auth-logo {
            max-width: 200px !important;
          }
        }
      `}</style>
    </div>
  )
}

export default AuthLayout
