let token = '';
let currentLessonId = '';

function showRegister() {
    document.getElementById('authSection').classList.add('hidden');
    document.getElementById('registerSection').classList.remove('hidden');
}

function showLogin() {
    document.getElementById('registerSection').classList.add('hidden');
    document.getElementById('authSection').classList.remove('hidden');
}

async function register() {
    const login = document.getElementById('regLoginInput').value;
    const password = document.getElementById('regPasswordInput').value;
    if (!login || !password) { alert("Заполните все поля!"); return; }
    const response = await fetch('https://localhost:7084/api/auth/register', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ login, password })
    });
    if (response.ok) { alert("Регистрация успешна!"); showLogin(); }
    else { alert("Ошибка регистрации."); }
}

async function login() {
    const login = document.getElementById('loginInput').value;
    const password = document.getElementById('passwordInput').value;
    if (!login || !password) { alert("Заполните все поля!"); return; }
    const response = await fetch('https://localhost:7084/api/auth/login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ login, password })
    });
    if (response.ok) {
        const data = await response.json();
        token = data.token;
        document.getElementById('authSection').classList.add('hidden');
        document.getElementById('mainSection').classList.remove('hidden');
        loadLanguages();
    } else { alert("Неверный логин или пароль"); }
}

async function loadLanguages() {
    const response = await fetch('https://localhost:7084/api/languages', {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (response.ok) {
        const languages = await response.json();
        const select = document.getElementById('languageSelect');
        select.innerHTML = '<option value="">Выберите язык...</option>';
        languages.forEach(lang => {
            const option = document.createElement('option');
            option.value = lang.id;
            option.text = lang.name;
            select.appendChild(option);
        });
    }
}

async function loadLessons() {
    const langId = document.getElementById('languageSelect').value;
    if (!langId) return;
    const response = await fetch(`https://localhost:7084/api/lessons/language/${langId}`, {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (response.ok) {
        const lessons = await response.json();
        const list = document.getElementById('lessonsList');
        list.innerHTML = '';
        lessons.forEach(lesson => {
            const li = document.createElement('li');
            li.className = 'list-group-item list-group-item-action';
            li.style.cursor = 'pointer';
            li.innerText = lesson.title;
            li.onclick = () => showLesson(lesson);
            list.appendChild(li);
        });
    }
}

function showLesson(lesson) {
    document.getElementById('noLessonSelected').classList.add('hidden');
    document.getElementById('lessonContent').classList.remove('hidden');
    currentLessonId = lesson.id;
    document.getElementById('lessonTitle').innerText = lesson.title;
    document.getElementById('lessonTheory').innerText = lesson.theory;
    loadChatHistory(lesson.id);
    if (lesson.tasks && lesson.tasks.length > 0) {
        document.getElementById('taskDescription').innerText = lesson.tasks[0].description;
        document.getElementById('currentTaskId').value = lesson.tasks[0].id;
    } else {
        document.getElementById('taskDescription').innerText = "Задачи не найдены.";
    }
}

function appendMessage(text, isUser) {
    const chatMessages = document.getElementById('chatMessages');
    const div = document.createElement('div');
    div.className = isUser ? 'msg-user' : 'msg-ai';
    div.innerText = text;
    chatMessages.appendChild(div);
    chatMessages.scrollTop = chatMessages.scrollHeight;
}

async function sendChatMessage() {
    const input = document.getElementById('chatInput');
    const text = input.value.trim();
    if (!text) return;
    appendMessage(text, true);
    input.value = '';
    const response = await fetch('https://localhost:7084/api/chat/message', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify({ lessonId: currentLessonId, text: text })
    });
    if (response.ok) {
        const data = await response.json();
        appendMessage(data.answer, false);
    } else { appendMessage("Ошибка при запросе к ИИ.", false); }
}

async function checkCode() {
    const code = document.getElementById('codeInput').value;
    if (!code) { alert("Напишите код!"); return; }
    appendMessage(`Вот мой код:\n${code}`, true);
    const response = await fetch('https://localhost:7084/api/chat/message', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json', 'Authorization': `Bearer ${token}` },
        body: JSON.stringify({ lessonId: currentLessonId, text: `Проверь мой код:\n${code}` })
    });
    if (response.ok) {
        const data = await response.json();
        appendMessage(data.answer, false);
    } else { appendMessage("Ошибка при запросе к ИИ.", false); }
}

function handleKeyPress(event) {
    if (event.key === 'Enter') { sendChatMessage(); }
}

async function loadChatHistory(lessonId) {
    const chatMessages = document.getElementById('chatMessages');
    chatMessages.innerHTML = '';
    const response = await fetch(`https://localhost:7084/api/chat/history/${lessonId}`, {
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (response.ok) {
        const history = await response.json();
        history.forEach(msg => {
            const isAi = msg.isFromAi !== undefined ? msg.isFromAi : msg.IsFromAi;
            const text = msg.text !== undefined ? msg.text : msg.Text;
            appendMessage(text, !isAi);
        });
    }
}

async function clearChat() {
    if (!confirm("Вы уверены, что хотите очистить историю?")) return;
    const response = await fetch(`https://localhost:7084/api/chat/history/${currentLessonId}`, {
        method: 'DELETE',
        headers: { 'Authorization': `Bearer ${token}` }
    });
    if (response.ok) {
        document.getElementById('chatMessages').innerHTML = '';
    } else {
        alert("Ошибка при очистке");
    }
}