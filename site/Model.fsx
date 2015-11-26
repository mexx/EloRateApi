namespace EloRateApi.Model

#load "Messages.fsx"
#load "Player.fsx"

open EloRateApi.Model.Player
open EloRateApi.Messages

type msg =
| CreatePlayer of CreatePlayerMessage * AsyncReplyChannel<PlayerResource>
| DeletePlayer of int
| UpdatePlayer of PlayerResource
| UpdatePlayerById of int * CreatePlayerMessage
| Fetch of AsyncReplyChannel<PlayerResource list>

type Model() =
    let innerModel =
        MailboxProcessor.Start(fun inbox ->
            let rec messageLoop players =
                async { let! msg = inbox.Receive()
                        match msg with
                        | CreatePlayer(p, replyChannel) ->
                            let newPlayer = Player.Create players p
                            replyChannel.Reply(newPlayer)
                            return! messageLoop(newPlayer::players)
                        | DeletePlayer(id) -> return! messageLoop (Player.Delete players id)
                        | UpdatePlayer(p) -> return! messageLoop (Player.Update players p)
                        | UpdatePlayerById (id, p) -> return! messageLoop (Player.UpdateById players id p)
                        | Fetch(replyChannel) ->
                            replyChannel.Reply(players)
                            return! messageLoop(players) }
            messageLoop [])

    member this.Players () =
        innerModel.PostAndReply(fun replyChannel -> Fetch replyChannel)

    member this.Player id =
        Seq.tryFind (fun (p: PlayerResource) -> p.Id = id) (this.Players ())

    member this.Create player =
        innerModel.PostAndReply(fun replyChannel -> CreatePlayer (player, replyChannel))

    member this.Delete id =
        innerModel.Post (DeletePlayer id)

    member this.Update player =
        innerModel.Post (UpdatePlayer player)
        this.Player player.Id

    member this.UpdateById (id: int) (player: CreatePlayerMessage) =
        innerModel.Post (UpdatePlayerById (id, player))
        this.Player id
