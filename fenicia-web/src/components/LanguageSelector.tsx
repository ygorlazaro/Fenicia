import React from 'react';
import { useTranslation } from 'react-i18next';
import { CDropdown, CDropdownToggle, CDropdownMenu, CDropdownItem } from '@coreui/react';

const LanguageSelector = () => {
  const { t, i18n } = useTranslation();

  const changeLanguage = (lng) => {
    i18n.changeLanguage(lng);
    localStorage.setItem('language', lng);
  };

  const getCurrentLanguageLabel = () => {
    switch (i18n.language) {
      case 'pt-BR':
        return 'PT-BR';
      case 'en':
        return 'EN';
      case 'es':
        return 'ES';
      default:
        return 'PT-BR';
    }
  };

  return (
    <CDropdown variant="nav-item">
      <CDropdownToggle caret={false}>
          {/*<CIcon icon={cilLanguage} title={t('language.title')} />*/}
          <span title={t('language.title')} />
        <span className="ms-2 d-none d-md-inline">{getCurrentLanguageLabel()}</span>
      </CDropdownToggle>
      <CDropdownMenu>
        <CDropdownItem 
          active={i18n.language === 'pt-BR'} 
          onClick={() => changeLanguage('pt-BR')}
        >
          {t('language.ptBR')}
        </CDropdownItem>
        <CDropdownItem 
          active={i18n.language === 'en'} 
          onClick={() => changeLanguage('en')}
        >
          {t('language.en')}
        </CDropdownItem>
        <CDropdownItem 
          active={i18n.language === 'es'} 
          onClick={() => changeLanguage('es')}
        >
          {t('language.es')}
        </CDropdownItem>
      </CDropdownMenu>
    </CDropdown>
  );
};

export default LanguageSelector;
