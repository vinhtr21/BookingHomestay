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
    var TxnRef = getParameterByName('vnp_TxnRef');
    var SecureHash = getParameterByName('vnp_SecureHash');
    var ResponseCode = getParameterByName('vnp_ResponseCode');
    var TransactionStatus = getParameterByName('vnp_TransactionStatus');

    var apiUrl = `https://localhost:44375/vnpay_return/${TxnRef}/${SecureHash}/${ResponseCode}/${TransactionStatus}`;
    makeAjaxRequest(apiUrl, 'GET', function(response) {
        var notificationDiv = document.getElementById('notification');

        var responseData = JSON.parse(response); // Để giả sử response trả về là JSON

        if (responseData) {
            notificationDiv.innerHTML = `
                  <div
                    style="
                      border-radius: 200px;
                      height: 200px;
                      width: 200px;
                      background: #f8faf5;
                      margin: 0 auto;
                    "
                  >
                    <i class="checkmark">✓</i>
                  </div>
                  <h1>THANH TOÁN THÀNH CÔNG</h1>
                  <p>
                    ${responseData.message} 
                  </p>
                  <a href="./index.html"><button class="btn btn-outline-success mt-5">Trở về Trang Chủ</button></a>
            `;
        } else {
            notificationDiv.innerHTML = 'Không có dữ liệu để hiển thị.';
        }
    }, function(error) {
        console.error('Error during Ajax request!');
        console.error(error);
    });
});

function getParameterByName(name, url) {
    if (!url) url = window.location.href;
    name = name.replace(/[\[\]]/g, '\\$&');
    var regex = new RegExp('[?&]' + name + '(=([^&#]*)|&|#|$)'),
        results = regex.exec(url);
    if (!results) return null;
    if (!results[2]) return '';
    return decodeURIComponent(results[2].replace(/\+/g, ' '));
}

