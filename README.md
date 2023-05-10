# Yuz_Tanimlama_Sistemi
- Kamerayı aç butonuna tıklandığında kameramız aktif hale geliyor ve uygulama ekranının sol tarafında Picturebox içerisine görüntüyü aktarıyor.
- Yüzü algıla butonuna bastığımızda açılan kamera ekranındaki görüntüde herhangi bir yüz bulunması halinde yüz algılanarak etrafı bir kare içerisine alınıyor. Daha sonra kare içerisine algılanan yüz hatları “yüz algıla” butonunun altındaki alana aktarılıyor.
- MaskedTextBox içerisine 11 haneli TC Kimlik numarası giriliyor. TC Kimlik numarası tanımlanmadığı durumda sistem algılanan yüzü kaydetmeyecektir ve hata ekranı size yansıtılacaktır.
- Kişi Ekle butonuna basıldığı zaman TC Kimlik numarası tanımlanan kişi C:\Users\sSsaLLasSs\Desktop\Yuz_Tanımlama_Sistemi\Yuz_Tanımlama\bin\Debug\YakalananResimler içerisine kişinin 10 tane yakalan görüntüsü aktarılacaktır.
- Aynı yüz farklı bir TC kimlik numarası ile kaydedildiğinde ise Kişiyi Tanı seçeneğine basıldığında kişinin yüz çerçevesi etrafında “TANINAMADI” yazısı olacak ve kişi tanınmayacaktır.
- Kişiyi tanı butonuna basıldığında ise daha önce kaydedilen kişinin TC kimlik numarası kişinin görüntüsünün etrafında gözükecektir. Eşleştirme için Kişiyi tanı butonunun altında bulunan 2 Picturebox’a görüntü yansıyacaktır. Sol taraftaki picturebox’un görüntüsü kameradan aldığı değerdir. Sağ taraftaki PictureBox’un görüntüsü ise önceden kaydedilen kişinin görüntüsüdür ve ikisi arasında eşleştirme yapılarak yüz tanınacaktır.
- Yeni kayıt oluştur butonuna basıldığında ise program açılıştaki haline dönecektir ve yeni kayıt için hazır hale gelecektir.
