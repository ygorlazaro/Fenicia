import { CFormLabel, CFormSelect } from "@coreui/react";
import { useTranslation } from "react-i18next";

interface FeniciaSelectProps {
  handleInputChange: (e: React.ChangeEvent<HTMLSelectElement>) => void;
  data: { id: string; name: string }[];
  loadingOptions?: boolean;
  value: string;
  label: string;
  id: string;
}

export function FeniciaSelect({
  handleInputChange,
  data,
  value,
  loadingOptions = false,
  label,
  id,
}: FeniciaSelectProps) {
  const { t } = useTranslation();

  return <>
    <CFormLabel htmlFor={id}>{t(label)} *</CFormLabel>
    <CFormSelect id={id} name={id} value={value} onChange={handleInputChange} disabled={loadingOptions} required>
      <option value="">{t('common.select')}...</option>
      {data.map(item => <option key={item.id} value={item.id}>
        {item.name}
      </option>)}
    </CFormSelect>
  </>;
}
