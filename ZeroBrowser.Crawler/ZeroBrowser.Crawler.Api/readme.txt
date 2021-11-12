Build Docker:

C:\Projects\Headless-Crawler\ZeroBrowser.Crawler> docker build -t 0browser-crawler:latest .

Run Docker:
docker run -itd --env ASPNETCORE_URLS=http://+:5010 -p 5010:5010 --restart always --memory="450m" --cpus=".5" --name 0browser-crawler 0browser-crawler:latest

Swagger Page:
http://localhost:5010/swagger/index.html

Build Docker from Github:
docker build https://github.com/ZeroBrowser/Headless-Crawler.git#main