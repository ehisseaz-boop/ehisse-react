document.addEventListener('DOMContentLoaded', function () {
    // Initialize cart from localStorage
    let cart = JSON.parse(localStorage.getItem('cart')) || [];
    let favs = JSON.parse(localStorage.getItem('favs')) || [];

    function updateCartBadge() {
        const badge = document.getElementById('cart-count-badge');
        if (badge) {
            const totalItems = cart.reduce((sum, item) => sum + item.quantity, 0);
            if (totalItems > 0) {
                badge.innerText = totalItems;
                badge.style.display = 'inline-block';
            } else {
                badge.style.display = 'none';
            }
        }
    }

    updateCartBadge();

    // Add to cart functionality
    const addToCartBtns = document.querySelectorAll('.add-to-cart');
    addToCartBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const id = this.getAttribute('data-id');
            const title = this.getAttribute('data-title');
            const price = parseFloat(this.getAttribute('data-price') || 0);

            // Check if item already in cart
            const existingItemIndex = cart.findIndex(item => item.id === id);
            if (existingItemIndex > -1) {
                cart[existingItemIndex].quantity += 1;
            } else {
                cart.push({ id, title, price, quantity: 1 });
            }

            localStorage.setItem('cart', JSON.stringify(cart));
            updateCartBadge();

            // Show success toast
            if (typeof Swal !== 'undefined') {
                Swal.fire({
                    toast: true,
                    position: 'top-end',
                    icon: 'success',
                    title: 'Səbətə əlavə edildi',
                    text: title,
                    showConfirmButton: false,
                    timer: 2000,
                    timerProgressBar: true
                });
            } else {
                alert('Məhsul səbətə əlavə edildi!');
            }
        });
    });

    // Add to fav functionality
    const addToFavBtns = document.querySelectorAll('.add-to-fav');
    addToFavBtns.forEach(btn => {
        btn.addEventListener('click', function (e) {
            e.preventDefault();
            const id = this.getAttribute('data-id');
            
            const icon = this.querySelector('i');

            const existingItemIndex = favs.findIndex(favId => favId === id);
            if (existingItemIndex > -1) {
                favs.splice(existingItemIndex, 1); // Remove from favs
                if(icon) {
                    icon.classList.remove('bi-heart-fill');
                    icon.classList.add('bi-heart');
                    icon.style.color = '';
                }
                
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'info',
                        title: 'Seçilmişlərdən silindi',
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
            } else {
                favs.push(id);
                if(icon) {
                    icon.classList.remove('bi-heart');
                    icon.classList.add('bi-heart-fill');
                    icon.style.color = 'red';
                }

                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        toast: true,
                        position: 'top-end',
                        icon: 'success',
                        title: 'Seçilmişlərə əlavə edildi',
                        showConfirmButton: false,
                        timer: 1500
                    });
                }
            }

            localStorage.setItem('favs', JSON.stringify(favs));
        });
    });
    
    // Check initial fav states
    addToFavBtns.forEach(btn => {
        const id = btn.getAttribute('data-id');
        const icon = btn.querySelector('i');
        if (favs.includes(id) && icon) {
            icon.classList.remove('bi-heart');
            icon.classList.add('bi-heart-fill');
            icon.style.color = 'red';
        }
    });

    // Header cart button click
    const headerCartBtn = document.getElementById('header-cart-btn');
    if (headerCartBtn) {
        headerCartBtn.addEventListener('click', function(e) {
            e.preventDefault();
            if (cart.length === 0) {
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        icon: 'info',
                        title: 'Səbətiniz boşdur',
                        text: 'Səbətə məhsul əlavə edib yenidən yoxlayın.',
                        confirmButtonText: 'Oldu'
                    });
                } else {
                    alert('Səbətiniz boşdur');
                }
            } else {
                let html = '<ul class="list-group list-group-flush text-start">';
                let total = 0;
                cart.forEach((item, index) => {
                    const itemTotal = item.price * item.quantity;
                    total += itemTotal;
                    html += `<li class="list-group-item d-flex justify-content-between align-items-center">
                                <div>
                                    <h6 class="my-0">${item.title || 'Məhsul'}</h6>
                                    <small class="text-muted">${item.quantity} x ${item.price} AZN</small>
                                </div>
                                <span class="text-muted">${itemTotal} AZN</span>
                            </li>`;
                });
                html += `<li class="list-group-item d-flex justify-content-between bg-light">
                            <span><strong>Cəmi</strong></span>
                            <strong>${total} AZN</strong>
                        </li></ul>`;
                        
                if (typeof Swal !== 'undefined') {
                    Swal.fire({
                        title: 'Səbətiniz',
                        html: html,
                        showCancelButton: true,
                        confirmButtonText: 'Sifariş et',
                        cancelButtonText: 'Səbəti Təmizlə',
                        cancelButtonColor: '#d33',
                    }).then((result) => {
                        if (result.isConfirmed) {
                            Swal.fire('Uğurlu', 'Sifarişiniz qəbul edildi! (Demo)', 'success');
                            cart = [];
                            localStorage.setItem('cart', JSON.stringify(cart));
                            updateCartBadge();
                        } else if (result.dismiss === Swal.DismissReason.cancel) {
                            cart = [];
                            localStorage.setItem('cart', JSON.stringify(cart));
                            updateCartBadge();
                            Swal.fire({
                                toast: true,
                                position: 'top-end',
                                icon: 'info',
                                title: 'Səbət təmizləndi',
                                showConfirmButton: false,
                                timer: 1500
                            });
                        }
                    });
                }
            }
        });
    }

    // Grid / List view toggle
    const gridViewBtnElement = document.querySelector('.bi-grid-fill');
    const gridViewBtn = gridViewBtnElement ? gridViewBtnElement.closest('.btn-outline') : null;
    
    const listViewBtnElement = document.querySelector('.bi-list');
    const listViewBtn = listViewBtnElement ? listViewBtnElement.closest('.btn-outline') : null;
    const productsGrid = document.querySelector('.products-grid');

    if (gridViewBtn && listViewBtn && productsGrid) {
        gridViewBtn.addEventListener('click', function(e) {
            e.preventDefault();
            gridViewBtn.classList.add('border-accent');
            gridViewBtn.querySelector('i').classList.add('text-accent');
            listViewBtn.classList.remove('border-accent');
            listViewBtn.querySelector('i').classList.remove('text-accent');
            
            productsGrid.style.display = 'grid';
            productsGrid.style.gridTemplateColumns = 'repeat(auto-fill, minmax(220px, 1fr))';
            
            Swal.fire({ toast: true, position: 'bottom-end', title: 'Qrid (Şəbəkə) görünüşü', showConfirmButton: false, timer: 1000 });
        });

        listViewBtn.addEventListener('click', function(e) {
            e.preventDefault();
            listViewBtn.classList.add('border-accent');
            listViewBtn.querySelector('i').classList.add('text-accent');
            gridViewBtn.classList.remove('border-accent');
            gridViewBtn.querySelector('i').classList.remove('text-accent');
            
            productsGrid.style.display = 'flex';
            productsGrid.style.flexDirection = 'column';
            
            Swal.fire({ toast: true, position: 'bottom-end', title: 'Siyahı görünüşü', showConfirmButton: false, timer: 1000 });
        });
    }

    // Bell notification icon
    const bellIcon = document.querySelector('.icon-btn .bi-bell');
    if (bellIcon) {
        bellIcon.closest('button').addEventListener('click', function(e) {
            e.preventDefault();
            Swal.fire({
                toast: true,
                position: 'top-end',
                icon: 'info',
                title: 'Yeni bildiriş yoxdur',
                showConfirmButton: false,
                timer: 1500
            });
        });
    }

    // Empty links #
    document.querySelectorAll('a[href="#"]').forEach(a => {
        a.addEventListener('click', function(e) {
            e.preventDefault();
            Swal.fire({
                toast: true,
                position: 'top',
                icon: 'info',
                title: 'Bu bölmə tezliklə aktiv olacaq',
                showConfirmButton: false,
                timer: 1500
            });
        });
    });

    // Sorting dropdown
    const sortContainer = document.getElementById('sort-dropdown-container');
    if (sortContainer) {
        sortContainer.style.cursor = 'pointer';
        sortContainer.addEventListener('click', function(e) {
            e.preventDefault();
            Swal.fire({
                title: 'Sırala',
                input: 'select',
                inputOptions: {
                    'views': 'Ən çox baxılan',
                    'newest': 'Ən yenilər',
                    'price_asc': 'Ucuzdan bahaya',
                    'price_desc': 'Bahadan ucuza'
                },
                inputPlaceholder: 'Sıralama növünü seçin',
                showCancelButton: true,
                confirmButtonText: 'Tətbiq et',
                cancelButtonText: 'Ləğv et'
            }).then((result) => {
                if (result.isConfirmed && result.value) {
                    const selectedText = result.value === 'views' ? 'Ən çox baxılan' :
                                         result.value === 'newest' ? 'Ən yenilər' :
                                         result.value === 'price_asc' ? 'Ucuzdan bahaya' : 'Bahadan ucuza';
                    sortContainer.querySelector('strong').innerText = selectedText;
                    Swal.fire({ toast: true, position: 'bottom-end', title: 'Sıralama dəyişdirildi', showConfirmButton: false, timer: 1000 });
                }
            });
        });
    }

    // Theme toggle
    const themeToggleBtn = document.getElementById('theme-toggle');
    const themeIcon = document.getElementById('theme-icon');
    
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme !== 'dark') {
        document.documentElement.setAttribute('data-theme', 'light');
        if (themeIcon) {
            themeIcon.classList.remove('bi-moon-stars');
            themeIcon.classList.add('bi-sun-fill');
        }
    } else {
        document.documentElement.removeAttribute('data-theme');
        if (themeIcon) {
            themeIcon.classList.remove('bi-sun-fill');
            themeIcon.classList.add('bi-moon-stars');
        }
    }

    if (themeToggleBtn) {
        themeToggleBtn.addEventListener('click', function(e) {
            e.preventDefault();
            const currentTheme = document.documentElement.getAttribute('data-theme');
            if (currentTheme === 'light') {
                document.documentElement.removeAttribute('data-theme');
                localStorage.setItem('theme', 'dark');
                if (themeIcon) {
                    themeIcon.classList.remove('bi-sun-fill');
                    themeIcon.classList.add('bi-moon-stars');
                }
            } else {
                document.documentElement.setAttribute('data-theme', 'light');
                localStorage.setItem('theme', 'light');
                if (themeIcon) {
                    themeIcon.classList.remove('bi-moon-stars');
                    themeIcon.classList.add('bi-sun-fill');
                }
            }
        });
    }

    // Dynamic search
    const searchInput = document.getElementById('mainSearchInput');
    const searchResults = document.getElementById('searchResults');
    let searchTimeout;

    if (searchInput && searchResults) {
        searchInput.addEventListener('input', function() {
            clearTimeout(searchTimeout);
            const query = this.value.trim();

            if (query.length < 2) {
                searchResults.style.display = 'none';
                return;
            }

            searchTimeout = setTimeout(() => {
                fetch(`/Ajax/Search?q=${encodeURIComponent(query)}`)
                    .then(res => res.json())
                    .then(data => {
                        searchResults.innerHTML = '';
                        if (data.length > 0) {
                            data.forEach(item => {
                                const div = document.createElement('a');
                                div.href = `/Home/Details/${item.id}`;
                                div.className = 'd-flex align-items-center p-2 text-decoration-none border-bottom';
                                div.style.color = 'var(--text-main)';
                                
                                div.innerHTML = `
                                    <img src="${item.image}" alt="${item.title}" style="width: 40px; height: 40px; object-fit: cover; border-radius: 5px; margin-right: 10px;" />
                                    <div class="flex-grow-1 overflow-hidden">
                                        <div class="text-truncate" style="font-weight: 500; font-size: 0.9rem;">${item.title}</div>
                                        <div class="text-muted" style="font-size: 0.8rem;">${item.brand} ${item.model}</div>
                                    </div>
                                    <div class="ms-2" style="font-weight: 600; color: var(--accent);">${item.price} ₼</div>
                                `;
                                
                                // Hover effect
                                div.addEventListener('mouseenter', () => div.style.backgroundColor = 'var(--bg-hover)');
                                div.addEventListener('mouseleave', () => div.style.backgroundColor = 'transparent');
                                
                                searchResults.appendChild(div);
                            });
                        } else {
                            searchResults.innerHTML = '<div class="p-3 text-center text-muted">Heç nə tapılmadı</div>';
                        }
                        searchResults.style.display = 'block';
                    })
                    .catch(err => {
                        console.error('Search error:', err);
                    });
            }, 300);
        });

        // Hide results when clicking outside
        document.addEventListener('click', function(e) {
            if (!searchInput.contains(e.target) && !searchResults.contains(e.target)) {
                searchResults.style.display = 'none';
            }
        });

        // Show results when focusing input if there's a query
        searchInput.addEventListener('focus', function() {
            if (this.value.trim().length >= 2 && searchResults.innerHTML.trim() !== '') {
                searchResults.style.display = 'block';
            }
        });
    }
});
