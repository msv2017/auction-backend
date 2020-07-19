# auction-backend

## Pre-requisites

The project uses _mongodb_ for persistance. If not installed on local machine please use official [installation guide](https://docs.mongodb.com/manual/installation/) to install it.

To start _mongodb_ instance
```
mongod --dbpath FOLDER
```
> where FOLDER could be any folder. It's where _mongodb_ will create databases and store the data.

The application expects to have _mongodb_ instance up and running on `mongodb://localhost:27017`.

It is hardcoded in settings here:
```
./Api/appsettings.json
```

## Run

First time one should build the application:
```
dotnet restore
dotnet build
```

And to run it:
```
dotnet run --project Api
```

## Endpoints

Once the application is up and running there is swagger interface available at `https://localhost:5001`

Available endpoints are:

Verb | Path | Description |
--- | --- | ---
POST | `/admin/initialize`| Initializes users, items and the auction databases with some data
GET | `/auction/items` | Return items on the auction. If item auction is finished, transfer the ownership to the top bidder
POST | `/auction/items/{itemId}` | Put item on the auction
DELETE | `/auction/items/{itemId}` | Remove item from the auction
POST | `/auction/items/{itemId}/bids` | Bid on item
DELETE | `/auction/items/{itemId}/bids` | Remove bid on item
POST | `/user/authenticate` | Authenticate user and return JWT token
GET | `/user/items` | Return user items
GET | `/user/auctionitems` | Return user auction items
GET | `/user/bids` | Return user bids

## Authentication

As there are no endpoints for user and item creation at the time, one should call `POST /admin/initialize`.
It will create few users and items and make one user `admin` the owener of the items.
Application uses JWT tokens to validate user requests.
So, any requests to the API but `/user/authenticate` should have `Authorization` in request's header.
To get the token one should call `/user/authenticate` with `username` and `password` in the request's body and response will be the token in base64 format.
It is quite convenient to use swagger interface to authorize as it has built-in functionality for that. There is a button `Authorize` on top right section of the interface, which clicked allows to enter the token in a format `Bearer <token_in_base64>`. After this all subsequent calls to the API will use this token.

## Front-end integration

Before any interaction with the API, front end should get the token, using user's credentials via `/user/authenticate`.
Having token, `/auction/*` and `/user/*` endpoints could be called providing token in the header.
There is special endpoint `/auction/items`. Despite being `GET` it also acts as auction manager - validating auctioned items on every call.
