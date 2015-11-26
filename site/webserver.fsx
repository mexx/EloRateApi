#I "../packages/FAKE/tools/"

#r "../packages/FAKE/tools/FakeLib.dll"
#r "../packages/Suave/lib/net40/Suave.dll"
#r "../packages/Newtonsoft.Json/lib/net40/Newtonsoft.Json.dll"


#load "Restful.fsx"
#load "Model.fsx"

open Fake
open System.Net
open System
open Suave
open Suave.Http
open Suave.Http.Successful
open Suave.Http.RequestErrors
open Suave.Web
open Suave.Types
open Newtonsoft.Json
open EloRateApi.Rest
open EloRateApi.Model

let model = new Model()
model.Create { Name="Sandra"; Points=1100; Retired=true } |> ignore
model.Create { Name="Richard"; Points=1100; Retired=false } |> ignore
model.Create { Name="Tom"; Points=1000; Retired=false } |> ignore
model.Create { Name="Mary"; Points=1000; Retired=false } |> ignore
model.Create { Name="Harry"; Points=1000; Retired=false } |> ignore
model.Create { Name="Jane"; Points=1000; Retired=false } |> ignore


let PlayerRoutes = choose [
                        Get "players" model.Players
                        GetById "players" model.Player
                        Delete "players" model.Delete
                        Post "players" model.Create
                        Put "players" model.Update
                        PutById "players" model.UpdateById
                        ]

let routes = choose [
              PlayerRoutes
              (NOT_FOUND "Huh?")
            ]

let serverConfig =
    let port = getBuildParamOrDefault "port" "8083" |> Sockets.Port.Parse
    { defaultConfig with bindings = [ HttpBinding.mk HTTP IPAddress.Loopback port ] }

startWebServer serverConfig routes
