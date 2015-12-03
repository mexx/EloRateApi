namespace EloRateApi.Model

open CQAgent

type PlayerResource = {
  Id : int
  Name : string
  Points : int
  Retired : bool
}

type GameResource = {
  Id: int
  Winner : string
  Loser : string
}


type State = { 
               Players: PlayerResource list;
               NextPlayerId: int; 
               Games: GameResource list;
               NextGameId: int
             }  

module Model =

    let initialState = { Players = []; NextPlayerId = 1; Games = []; NextGameId = 1 }



[<AutoOpen>]
module Player =
    let getPlayerById id = Seq.tryFind (fun (p: PlayerResource) -> p.Id = id)

    let removePlayer id = List.filter (fun (p: PlayerResource) -> p.Id <> id)



    let public GetAll (model: CQAgent<State>) () =
        model.Query (fun (s: State) -> s.Players)

    let public GetItem (model: CQAgent<State>) id =
        model.Query (fun (s: State) -> getPlayerById id s.Players)

    let public Create (model: CQAgent<State>) (player: PlayerResource) =
        model.Command (fun (s: State) ->
            let newPlayer = {player with Id = s.NextPlayerId} 
            (newPlayer, {s with Players = newPlayer::s.Players; 
                                NextPlayerId = s.NextPlayerId + 1}))

    let public DeleteItem (model: CQAgent<State>) id =
        model.Command (fun (s: State) ->
            ((), { s with Players = removePlayer id s.Players }))

    let public Update (model: CQAgent<State>) (updatedPlayer: PlayerResource) =
        model.Command (fun (s: State) ->
            let existing = getPlayerById updatedPlayer.Id s.Players
            match existing with
            | Some e -> (Some updatedPlayer, {s with Players = updatedPlayer::removePlayer updatedPlayer.Id s.Players })
            | None -> (None, s))

    let public UpdateById (model: CQAgent<State>) id (player: PlayerResource) =
        model.Command (fun (s: State) ->
            let existing = getPlayerById id s.Players
            match existing with
            | Some e ->
                    let newPlayer = {player with Id = e.Id} 
                    (Some newPlayer, { s with Players = newPlayer::removePlayer e.Id s.Players })
            | None -> (None, s))





[<AutoOpen>]
module Game =

    let getGameById id = Seq.tryFind (fun (g: GameResource) -> g.Id = id)

    let removeGame id = List.filter (fun (g: GameResource) -> g.Id <> id)



    let public GetAll (model: CQAgent<State>) () =
        model.Query (fun (s: State) -> s.Games)

    let public GetItem (model: CQAgent<State>) id =
        model.Query (fun (s: State) -> getGameById id s.Games)

    let public Create (model: CQAgent<State>) (game: GameResource) =
        model.Command (fun (s: State) ->
            let newGame = {game with Id = s.NextGameId} 
            (newGame, {s with Games = newGame::s.Games; 
                              NextGameId = s.NextGameId + 1}))

    let public DeleteItem (model: CQAgent<State>) id =
        model.Command (fun (s: State) ->
            ((), { s with Games = removeGame id s.Games }))