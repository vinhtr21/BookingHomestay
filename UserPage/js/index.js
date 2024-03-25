function makeAjaxRequest(url, method, callback) {
    var xhr = new XMLHttpRequest();
    xhr.open(method, url, true);
    xhr.onreadystatechange = function() {
        if (xhr.readyState === 4 && xhr.status === 200) {
            callback(xhr.responseText);
        }
    };
    xhr.send();
}

$(document).ready(() => {
    var apiUrl = `https://localhost:44375/api/Filter/filterAll`;
    makeAjaxRequest(apiUrl, 'GET', function(response) {
        var filter = document.getElementById('filter');

        var responseData = JSON.parse(response); // Để giả sử response trả về là JSON
        console.log(responseData);
        responseData.forEach(element => {
            var html =`
            <div class="col-lg-3 col-md-6">
        
              <div class="hr-text">
                <h3>${element.typeName}</h3>
                <table>
                  <tbody>
                    <tr>
                      <td class="r-o">Kích thước:</td>
                      <td>${element.typeSize}</td>
                    </tr>
                    <tr>
                      <td class="r-o">Số phòng:</td>
                      <td>${element.typeBedRoom}</td>
                    </tr>
                    <tr>
                      <td class="r-o">Dịch vụ:</td>
                      <td>${element.typeService}</td>
                    </tr>
                  </tbody>
                </table>
                <a href="./rooms.html?filter=${element.typeId}" class="primary-btn"
                  >Xem Chi Tiết</a
                >
              </div>
            </div>
          </div>
            `;
            filter.insertAdjacentHTML('beforeend', html);
        });

    });
});

