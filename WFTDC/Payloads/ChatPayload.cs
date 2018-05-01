namespace WFTDC.Payloads.Chat
{
    using System;
    using System.Collections.Generic;
    using J = Newtonsoft.Json.JsonPropertyAttribute;

    public class ChatPayload
    {
        [J("payload")] public Payload Payload { get; set; }
    }

    public class Payload
    {
        [J("chats")] public List<Chat> Chats { get; set; }
    }

    public class Chat
    {
        [J("id")] public string Id { get; set; }
        [J("unread_count")] public long UnreadCount { get; set; }
        [J("chat_with")] public List<ChatWith> ChatWith { get; set; }
        [J("last_update")] public DateTimeOffset LastUpdate { get; set; }
        [J("messages")] public List<Message> Messages { get; set; }
        [J("chat_name")] public string ChatName { get; set; }
    }

    public class ChatWith
    {
        [J("id")] public string Id { get; set; }
        [J("ingame_name")] public string IngameName { get; set; }
        [J("avatar")] public string Avatar { get; set; }
        [J("reputation")] public long Reputation { get; set; }
        [J("region")] public Region Region { get; set; }
        [J("status")] public Status Status { get; set; }
    }

    public class Message
    {
        [J("send_date")] public DateTimeOffset SendDate { get; set; }
        [J("id")] public string Id { get; set; }
        [J("chat_id")] public string ChatId { get; set; }
        [J("message")] public string MessageMessage { get; set; }
        [J("message_from")] public string MessageFrom { get; set; }
    }

    public enum Region { En };
}
