# test image endpoint

```bash
curl 'http://localhost:7071/api/Image?name=Acne1.png' \
  -H 'sec-ch-ua-platform: "macOS"' \
  -H 'Referer: https://mbdstoragesa.z14.web.core.windows.net/' \
  -H 'User-Agent: Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36' \
  -H 'Accept: */*' \
  -H 'sec-ch-ua: "Chromium";v="130", "Google Chrome";v="130", "Not?A_Brand";v="99"' \
  -H 'Content-Type: multipart/form-data; boundary=----WebKitFormBoundaryCLseAAB5y3vrP1fG' \
  -H 'sec-ch-ua-mobile: ?0' \
  --data-raw $'------WebKitFormBoundaryCLseAAB5y3vrP1fG\r\nContent-Disposition: form-data; name="File"; filename="Acne1.png"\r\nContent-Type: image/png\r\n\r\n\r\n------WebKitFormBoundaryCLseAAB5y3vrP1fG--\r\n'
```

```bash

curl 'https://mbdstoragesa.blob.core.windows.net/mbd-images/Accidents2.png' \
  -H 'Referer: https://mbdstoragesa.z14.web.core.windows.net/' 
```

``` bash
#fetch ailment 
curl http://localhost:7071/api/Ailment?&id=6e10e347-eab2-4b19-90e1-c4ddd9c9c845&name=test1 | jq
```
