namespace EloRateApi.Model.Player

#load "Messages.fsx"

open EloRateApi.Messages

type PlayerResource = {
  Id : int
  Name : string
  Points : int
  Retired : bool
}

[<AutoOpen>]
module Player =

    let NewPlayer players id (message: CreatePlayerMessage) =
        {
        Id = id
        Name = message.Name
        Points = message.Points
        Retired = message.Retired
        }


    let public GetById players id =
        Seq.tryFind (fun (p: PlayerResource) -> p.Id = id) players


    let public Create players (message: CreatePlayerMessage) =
        NewPlayer players (1 + List.length players) message


    let public Delete players id =
        List.filter (fun p -> p.Id <> id) players


    let public Update players (updatedPlayer: PlayerResource) =
        let existing = GetById players updatedPlayer.Id
        match existing with
        | Some e -> updatedPlayer::Delete players updatedPlayer.Id
        | None -> players


    let public UpdateById players id (message: CreatePlayerMessage) =
        let existing = GetById players id
        match existing with
        | Some _ -> NewPlayer players id message::Delete players id
        | None -> players
