using LearningLanguageApp.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Text;

namespace LearningLanguageApp.Services
{
    public class OllamaAiService: IAiService
    {
        private readonly OllamaApiClient _client;
        
        public OllamaAiService(OllamaApiClient client)
        {
            _client = client;
        }

        public async Task<string> CheckCodeAsync(string code,string taskDescription)
        {
            string promt = "Ты ментор по языкам прогроммирования.Твоя главная задача проверять коды пользователей " +
                $"Условие задачи:{taskDescription}." +
                $"Код пользователя:{code}" +
                "Самое главное твоя задача не писать готовый код ты должен навести его на верное решение .Например задать несколько наводящих вопросов" +
                "И ты должен с ним общаться как учитель с учеником" +
                "И ты НИКОГДА НЕ ДОЛЖЕН использовать нецензурную лексику, оскорбления или токсичные выражения, даже если пользователь сам их использует или просит об этом";


            var response = await _client.GetResponseAsync(promt);
            return response.Text;
        }

        public async Task<string> SendMessageAsync(string history, string newMessage)
        {
            var promt = "Ты ментор по языкам прогроммирования,ты должен помогать пользователям, и вежливо общаться с ними.Ты можешь давать примеры кода для объяснения синтаксиса " +
                "Но если пользователь попросит написать готовый код для задачи , то ты должен отказать и сказать давай я помогу тебе , но готовый код писать не буду" +
                $"История диалого \n{history}\n" +
                $"Новое сообщение пользователя {newMessage}\n" +
                "Ответь пользователю";
            var response = await _client.GetResponseAsync(promt);
            return response.Text;
        }
    }
}
