// Sevimlilər (Favorites) idarəetmə köməkçisi

const STORAGE_KEY = 'ehisse_favorites';

export function getFavorites() {
  try {
    const raw = localStorage.getItem(STORAGE_KEY);
    return raw ? JSON.parse(raw) : [];
  } catch {
    return [];
  }
}

export function isFavorite(id) {
  if (!id) return false;
  const list = getFavorites();
  return list.some(item => (typeof item === 'object' ? item.id : item) === id);
}

export function toggleFavorite(part) {
  if (!part || !part.id) return false;
  const list = getFavorites();
  const existingIdx = list.findIndex(item => (typeof item === 'object' ? item.id : item) === part.id);
  
  let added = false;
  if (existingIdx >= 0) {
    list.splice(existingIdx, 1);
    added = false;
  } else {
    // Saxlayırıq ki, sevimlilər səhifəsində server sorğusuna ehtiyac qalmadan da sürətlə açılsın
    list.unshift({
      id: part.id,
      title: part.title,
      price: part.price,
      currency: part.currency || 'AZN',
      stockQuantity: part.stockQuantity,
      partTypeName: part.partTypeName,
      qualityTypeName: part.qualityTypeName,
      sellerName: part.sellerName,
      modelName: part.modelName,
      brandName: part.brandName,
      images: part.images || [],
      createdDate: part.createdDate || new Date().toISOString()
    });
    added = true;
  }
  
  localStorage.setItem(STORAGE_KEY, JSON.stringify(list));
  // Event yayırıq ki, bütün səhifələr və header/navbar anında sayğacı yeniləsin
  window.dispatchEvent(new Event('favorites-updated'));
  return added;
}

export function getFavoritesCount() {
  return getFavorites().length;
}
