import React, { useState, useEffect } from 'react'
import { CButton, CModal, CModalBody, CModalHeader, CModalTitle, CModalFooter } from '@coreui/react'
import CIcon from '@coreui/icons-react'
import { cilCloudDownload } from '@coreui/icons'

const PWAInstallPrompt = () => {
  const [deferredPrompt, setDeferredPrompt] = useState(null)
  const [showInstallPrompt, setShowInstallPrompt] = useState(false)

  useEffect(() => {
    const handleBeforeInstallPrompt = (e) => {
      // Prevent the mini-infobar from appearing on mobile
      e.preventDefault()
      // Store the event for later use
      setDeferredPrompt(e)
      // Show the install prompt after a delay
      setTimeout(() => {
        setShowInstallPrompt(true)
      }, 30000) // Show after 30 seconds
    }

    const handleAppInstalled = () => {
      setShowInstallPrompt(false)
      setDeferredPrompt(null)
    }

    window.addEventListener('beforeinstallprompt', handleBeforeInstallPrompt)
    window.addEventListener('appinstalled', handleAppInstalled)

    return () => {
      window.removeEventListener('beforeinstallprompt', handleBeforeInstallPrompt)
      window.removeEventListener('appinstalled', handleAppInstalled)
    }
  }, [])

  const handleInstallClick = async () => {
    if (!deferredPrompt) {
      return
    }

    // Show the install prompt
    deferredPrompt.prompt()
    
    // Wait for the user to respond to the prompt
    const { outcome } = await deferredPrompt.userChoice
    console.log(`User response to the install prompt: ${outcome}`)
    
    // Reset the deferred prompt variable
    setDeferredPrompt(null)
    setShowInstallPrompt(false)
  }

  const handleClose = () => {
    setShowInstallPrompt(false)
  }

  if (!showInstallPrompt) {
    return null
  }

  return (
    <CModal visible={showInstallPrompt} onClose={handleClose} size="lg">
      <CModalHeader>
        <CModalTitle>
          <CIcon icon={cilCloudDownload} className="me-2" />
          Install Fenicia App
        </CModalTitle>
      </CModalHeader>
      <CModalBody>
        <p>
          Install Fenicia - Gato Ninja Site as an app on your device for faster access and a better
          experience. This allows you to use the app offline and get quick access from your home
          screen.
        </p>
        <ul className="mt-3">
          <li>✓ Works offline</li>
          <li>✓ Fast and responsive</li>
          <li>✓ Easy access from home screen</li>
          <li>✓ Automatic updates</li>
        </ul>
      </CModalBody>
      <CModalFooter>
        <CButton color="secondary" onClick={handleClose}>
          Later
        </CButton>
        <CButton color="primary" onClick={handleInstallClick}>
          <CIcon icon={cilCloudDownload} className="me-2" />
          Install Now
        </CButton>
      </CModalFooter>
    </CModal>
  )
}

export default PWAInstallPrompt
