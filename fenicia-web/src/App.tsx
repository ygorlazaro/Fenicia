import React, { Suspense, useEffect } from 'react'
import { HashRouter, Route, Routes } from 'react-router-dom'

import { CSpinner, useColorModes } from '@coreui/react'
import './scss/style.scss'

// We use those styles to show code examples, you should remove them in your application.
import './scss/examples.scss'

// Containers
const DefaultLayout = React.lazy(() => import('./layout/DefaultLayout'))

// Pages
const AuthLogin = React.lazy(() => import('./views/auth/login'))
const AuthRegister = React.lazy(() => import('./views/auth/register'))
const ForgotPassword = React.lazy(() => import('./views/auth/forgot-password'))
const Page404 = React.lazy(() => import('./views/pages/page404/Page404'))
const Page500 = React.lazy(() => import('./views/pages/page500/Page500'))

const App = () => {
  const { setColorMode } = useColorModes('fenicia-gato-ninja-theme')

  useEffect(() => {
    // Set dark mode as default
    setColorMode('dark')
  }, [setColorMode])

  return (
    <HashRouter>
      <Suspense
        fallback={
          <div className="pt-3 text-center">
            <CSpinner color="primary" variant="grow" />
          </div>
        }
      >
        <Routes>
          <Route exact path="/auth/login" name="Login Page" element={<AuthLogin />} />
            <Route exact path="/auth/register" name="Register Page" element={<AuthRegister />} />
            <Route exact path="/auth/forgot-password" name="Forgot Password" element={<ForgotPassword />} />
          <Route exact path="/404" name="Page 404" element={<Page404 />} />
          <Route exact path="/500" name="Page 500" element={<Page500 />} />
          <Route path="*" name="Home" element={<DefaultLayout />} />
        </Routes>
      </Suspense>
    </HashRouter>
  )
}

export default App
