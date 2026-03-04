import React from 'react'
import { CFooter } from '@coreui/react'

const AppFooter = () => {
  return (
    <CFooter className="px-4">
      <div>
        <a href="https://fenicia.gatoninja.com.br" target="_blank" rel="noopener noreferrer">
          Fenicia @ Gato Ninja Site
        </a>
        <span className="ms-1">&copy; 2025 Gato Ninja.</span>
      </div>
      <div className="ms-auto">
        <span className="me-1">Powered By</span>
        <a href="mailto:fenicia@gatoninja.com.br">
          Ninja Power (R) Ygor Lazaro
        </a>
      </div>
    </CFooter>
  )
}

export default React.memo(AppFooter)
