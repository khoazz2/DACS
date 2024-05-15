function filterStores() {
  // Lấy giá trị đã chọn
  const selectedArea = document.querySelector('select[name="data-map-areas"]').value;
    const selectedCity = document.querySelector('select[name="data-map-citys"]').value;
    const selectedDistrict = document.querySelector('select[name="data-map-districts"]').value;
    const selectedBrand = document.querySelector('select[name="data-types"]').value;

    // Lấy tất cả các mục cửa hàng
    const storeItems = document.querySelectorAll('.stores-result .store-item');

    // Duyệt qua từng mục cửa hàng
    storeItems.forEach(function(storeItem) {
    const areaClass = storeItem.classList.contains(selectedArea.slice(1)); // Xóa "." đứng đầu tên lớp
    const cityClass = storeItem.classList.contains(selectedCity.slice(1));
    const districtClass = storeItem.classList.contains(selectedDistrict.slice(1));
    const brandClass = storeItem.classList.contains(selectedBrand.slice(1));

    // Hiển thị cửa hàng nếu tất cả các bộ lọc khớp
    storeItem.style.display = (areaClass && cityClass && districtClass && brandClass) ? 'block' : 'none';
  });
}

    // Gọi hàm lọc khi thay đổi lựa chọn
    const filterSelects = document.querySelectorAll('.stores-filter select');
    filterSelects.forEach(function(select) {
        select.addEventListener('change', filterStores);
});

    // Gọi hàm lọc ban đầu để xử lý các tùy chọn được chọn trước (nếu có)
    filterStores();