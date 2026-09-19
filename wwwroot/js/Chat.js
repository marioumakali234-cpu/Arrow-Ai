const messageForm = document.getElementById("messageForm");

messageForm.addEventListener("submit", function (event) {

    event.preventDefault();

    sendMessage();

});


async function sendMessage() {

    const input = document.getElementById("messageInput");

    const message = input.value.trim();

    if (message === "") {
        return;
    }

    addUserMessage(message);

    input.value = "";

    try {

        const response = await fetch("/Chat/Send", {

            method: "POST",

            headers: {
                "Content-Type": "application/json"
            },

            body: JSON.stringify({
                message: message
            })

        });


        const data = await response.json();


        if (!response.ok) {

            addAIMessage(
                "Error: " + (data.error || "Something went wrong.")
            );

            return;
        }


        addAIMessage(data.response);

    }
    catch (error) {

        addAIMessage(
            "Connection error. Please try again."
        );

        console.error(error);

    }

}


function addUserMessage(message) {

    const chatBox = document.getElementById("chatBox");

    const messageDiv = document.createElement("div");

    messageDiv.className = "message user-message";

    messageDiv.innerHTML = `    <div class="message-name">You</div>
    <div class="message-text">${message}</div>
`;
          

    chatBox.appendChild(messageDiv);

    chatBox.scrollTop = chatBox.scrollHeight;

}


function addAIMessage(message) {

    const chatBox = document.getElementById("chatBox");

    const messageDiv = document.createElement("div");

    messageDiv.className = "message ai-message";

    messageDiv.innerHTML =
        `<div class="message-name">Arrow AI</div>
        <div class="message-text">${message}</div>
       `;  

    chatBox.appendChild(messageDiv);

    chatBox.scrollTop = chatBox.scrollHeight;

}