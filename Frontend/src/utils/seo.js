import { useEffect } from 'react';

/**
 * Hər səhifədə dinamik Meta Title və Meta Description təyin etmək üçün köməkçi hook
 */
export function usePageSeo({ title, description }) {
  useEffect(() => {
    if (title) {
      document.title = `${title} | e-hisse.az`;
    } else {
      document.title = 'e-hisse.az - Doğru hissəni tez tap.';
    }

    if (description) {
      let metaDesc = document.querySelector('meta[name="description"]');
      if (!metaDesc) {
        metaDesc = document.createElement('meta');
        metaDesc.setAttribute('name', 'description');
        document.head.appendChild(metaDesc);
      }
      metaDesc.setAttribute('content', description);
    }
  }, [title, description]);
}
