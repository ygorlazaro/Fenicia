# Fenicia PWA (Progressive Web App)

Fenicia now supports Progressive Web App (PWA) functionality, allowing users to install the application on their devices for offline access and improved performance.

## Features

- ✅ **Offline Support**: Works without an internet connection
- ✅ **Install Prompt**: Users can install the app on their home screen
- ✅ **Fast Loading**: Cached assets for quick load times
- ✅ **Auto Updates**: Service worker automatically updates cached content
- ✅ **Cross-Platform**: Works on desktop and mobile devices
- ✅ **iOS Support**: Safari on iOS supports PWA features

## PWA Components

### Service Worker
The service worker is automatically registered and handles:
- Caching of static assets (JS, CSS, HTML, images, fonts)
- Offline functionality
- Background updates

### Install Prompt
A modal dialog that appears after 30 seconds, prompting users to install the app. Features:
- Clean, user-friendly interface
- Explains benefits of installation
- Can be dismissed and shown later
- Automatically hides after installation

## Configuration

### vite.config.mjs
PWA is configured in `vite.config.mjs` using `vite-plugin-pwa`:

```javascript
VitePWA({
  registerType: 'autoUpdate',
  manifest: {
    name: 'Fenicia - Gato Ninja Site',
    short_name: 'Fenicia',
    // ... other configuration
  }
})
```

### Caching Strategy
- **Fonts**: CacheFirst (1 year)
- **Images**: CacheFirst (30 days)
- **JS/CSS**: StaleWhileRevalidate (7 days)

## Testing PWA

### Local Development
```bash
npm run build
npm run serve
```

Visit `http://localhost:4173` (production preview server)

### Chrome DevTools
1. Open DevTools (F12)
2. Go to Application tab
3. Check:
   - Manifest (should show Fenicia app info)
   - Service Workers (should show active worker)
   - Cache Storage (should show cached assets)

### Install Test
1. Build the app: `npm run build`
2. Serve: `npm run serve`
3. Wait 30 seconds for install prompt
4. Or click the install icon in the address bar

## Manual Installation

### Desktop (Chrome/Edge)
1. Look for the install icon (⊕) in the address bar
2. Click "Install"
3. App will open in a standalone window

### Mobile (Android Chrome)
1. Tap the menu (⋮)
2. Tap "Add to Home screen"
3. Confirm installation

### iOS (Safari)
1. Tap the Share button
2. Scroll down and tap "Add to Home Screen"
3. Tap "Add" in the top right corner

## Updating PWA Icons

To update the PWA icons, replace these files in `/public`:
- `pwa-192x192.svg` - 192x192 icon
- `pwa-512x512.svg` - 512x512 icon
- `apple-touch-icon.svg` - iOS icon (180x180)

For PNG icons, update `vite.config.mjs` and `manifest.json` accordingly.

## Troubleshooting

### Service Worker Not Registering
- Ensure you're using HTTPS (or localhost)
- Check browser console for errors
- Clear browser cache and reload

### Install Prompt Not Showing
- Must be served over HTTPS
- User must have visited the site at least twice
- Must have a valid service worker
- Check `beforeinstallprompt` event in console

### Offline Not Working
- Ensure service worker is active
- Check Cache Storage in DevTools
- Verify assets are being cached

## Browser Support

| Browser | Support |
|---------|---------|
| Chrome (Android/Desktop) | ✅ Full |
| Edge | ✅ Full |
| Firefox | ✅ Full |
| Safari (iOS/macOS) | ✅ Partial* |
| Opera | ✅ Full |

*Safari has limited PWA support but works as a web app

## Production Deployment

For production, ensure:
1. HTTPS is enabled
2. Service worker scope is correct
3. Manifest is accessible
4. Icons are properly sized

## More Information

- [Vite PWA Plugin Docs](https://vite-pwa-org.netlify.app/)
- [Web App Manifest](https://developer.mozilla.org/en-US/docs/Web/Manifest)
- [Service Workers](https://developer.mozilla.org/en-US/docs/Web/API/Service_Worker_API)
