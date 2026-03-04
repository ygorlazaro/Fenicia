/**
 * Service Worker Registration Utility for Fenicia PWA
 * 
 * This file provides utilities for registering and managing the service worker
 */

export const registerServiceWorker = async () => {
  if ('serviceWorker' in navigator) {
    try {
      const registration = await navigator.serviceWorker.register('/sw.js', {
        scope: '/',
      })
      
      if (registration.installing) {
        console.log('Service worker installing')
      } else if (registration.active) {
        console.log('Service worker active:', registration.active.state)
      } else if (registration.waiting) {
        console.log('Service worker waiting')
      }
      
      // Check for updates
      registration.addEventListener('updatefound', () => {
        const newWorker = registration.installing
        if (newWorker) {
          newWorker.addEventListener('statechange', () => {
            if (newWorker.state === 'installed' && navigator.serviceWorker.controller) {
              // New content is available, show update notification
              console.log('New content available, please refresh.')
              if (confirm('New version available! Reload to update?')) {
                window.location.reload()
              }
            }
          })
        }
      })
      
      return registration
    } catch (error) {
      console.error('Service worker registration failed:', error)
      return null
    }
  }
  return null
}

export const unregisterServiceWorker = async () => {
  if ('serviceWorker' in navigator) {
    const registrations = await navigator.serviceWorker.getRegistrations()
    for (const registration of registrations) {
      await registration.unregister()
    }
    console.log('Service workers unregistered')
  }
}

export const checkServiceWorkerStatus = async () => {
  if ('serviceWorker' in navigator) {
    const registrations = await navigator.serviceWorker.getRegistrations()
    return {
      supported: true,
      registered: registrations.length > 0,
      count: registrations.length,
    }
  }
  return {
    supported: false,
    registered: false,
    count: 0,
  }
}

export default registerServiceWorker
