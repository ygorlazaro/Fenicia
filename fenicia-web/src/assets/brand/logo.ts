// Fenicia logo - using SVG import
import feniciaLogo from './fenicia.svg'

// Export as array format for compatibility with existing components
export const logo = [
  '100% 100%',
  `<image href="${feniciaLogo}" width="100%" height="100%" preserveAspectRatio="xMidYMid meet" />`
]

// Also export the direct SVG path for new components
export const logoPath = feniciaLogo
