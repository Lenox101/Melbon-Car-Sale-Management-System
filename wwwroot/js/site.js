// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

document.addEventListener('DOMContentLoaded', function() {
    // Smooth scroll for anchor links
    document.querySelectorAll('a[href^="#"]').forEach(anchor => {
        anchor.addEventListener('click', function (e) {
            e.preventDefault();
            const targetId = this.getAttribute('href');
            if (targetId === '#') return;
            
            const target = document.querySelector(targetId);
            if (target) {
                const navHeight = document.querySelector('.navbar').offsetHeight;
                const targetPosition = target.getBoundingClientRect().top + window.pageYOffset;
                
                window.scrollTo({
                    top: targetPosition - navHeight - 20, // 20px extra padding
                    behavior: 'smooth'
                });

                // Close mobile menu if open
                const navbarCollapse = document.querySelector('.navbar-collapse');
                if (navbarCollapse.classList.contains('show')) {
                    document.querySelector('.navbar-toggler').click();
                }
            }
        });
    });

    var contactPopup = document.getElementById("contactPopup");
    if (contactPopup) {
        const contactButton = document.querySelector('.btn-primary[data-bs-toggle="modal"]');
        const closeButton = contactPopup.querySelector('.close-btn');

        contactButton.addEventListener('click', function () {
            contactPopup.classList.remove('d-none');
        });

        closeButton.addEventListener('click', function () {
            contactPopup.classList.add('d-none');
        });

        // Optional: Close the pop-up when clicking outside of it
        window.addEventListener('click', function (event) {
            if (event.target === contactPopup) {
                contactPopup.classList.add('d-none');
            }
        });
    }

    const makeSelect = document.getElementById('makeSelect');
    const modelSelect = document.getElementById('modelSelect');
    const priceRangeSelect = document.getElementById('priceRangeSelect');

    // Set selected price range if it exists in URL
    const urlParams = new URLSearchParams(window.location.search);
    const searchPriceRange = urlParams.get('searchPriceRange');
    if (searchPriceRange) {
        priceRangeSelect.value = searchPriceRange;
    }

    if (makeSelect && modelSelect) {
        makeSelect.addEventListener('change', function() {
            const selectedMake = this.value;
            modelSelect.innerHTML = '<option value="">Select Model</option>';

            if (selectedMake) {
                fetch(`/Home/GetModels?make=${selectedMake}`)
                    .then(response => response.json())
                    .then(models => {
                        models.forEach(model => {
                            const option = document.createElement('option');
                            option.value = model;
                            option.textContent = model;
                            modelSelect.appendChild(option);
                        });
                    })
                    .catch(error => console.error('Error:', error));
            }
        });

        // Trigger change event if make is selected
        if (makeSelect.value) {
            makeSelect.dispatchEvent(new Event('change'));
        }
    }
});

function openPopup(carId) {
    const popup = document.getElementById(`carPopup-${carId}`);
    popup.classList.add('show');
    document.body.style.overflow = 'hidden';
}

function closePopup(carId) {
    const popup = document.getElementById(`carPopup-${carId}`);
    popup.classList.remove('show');
    document.body.style.overflow = '';
}

// Close popup when clicking outside
document.addEventListener('click', function(event) {
    if (event.target.classList.contains('car-popup')) {
        const carId = event.target.id.split('-')[1];
        closePopup(carId);
    }
});

// Close popup when pressing ESC key
document.addEventListener('keydown', function(event) {
    if (event.key === 'Escape') {
        const openPopup = document.querySelector('.car-popup.show');
        if (openPopup) {
            const carId = openPopup.id.split('-')[1];
            closePopup(carId);
        }
    }
});
