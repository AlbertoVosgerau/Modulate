Unity Modulate!

This is a small personal architecture project that solves many of problems I've encountered as a solo or game dev or while working with small teams.

Even for a small project, architecture is a key part and having a solid and well defined structure that is designed to reinforce good practices and organization is key for preventing bugs and ensuring your own sanity at times.
If you want to put a good small project or prototype quiclkly, it's a good idea to have this taken care and working for you and releasing you for creativity instead!

This package relies heavily on code generation and a resourceful set of tools that can make your small project to start in no time.
With Modulate! you can have a project architecture setup in seconds.

And what's even better, a modular philosophy ensures your code won't turn into spaghetti, while a Event Bus gives you freedom, flexibiliy and avoids strong references. On top of that, a feature toggle enables you to turn features on or off with a single click.
This is meant to be generic, so there's just enough order so you can't mess this up while testing and exploring, but not so much as to stop you from doing whatever you need. Keep that in mind, as many decisions are left open on purpose so you can fit whatever game you have in mind in it.

Let's take a look how it works!


Architecture structure

Much like architectures like MVVM or MVC, there's a clear distinction of where business logic, models and actual rendering or gameplay.
In Modulate! a clear line is drawn between Managers and GameServices.

In Modulate! a Manager is the part of the architecture that deals with Unity runtime content, gameplay, rendering and view in general. It takes logic that's local to your game loop. A Manager is NOT a Monobehaviour, but it's instanced as an object on the ManagerContainer gameObject.
The ManagerContainer lifecycle is managed by Modulate! Main class, and the Manager object is the only part of this architeture designed to interact with Unity ever. It's higly advisable to use Modulate!'s built in Event Bus to comunicate between Monobehaviour classes and the Manager. This is not inforced as the goal of this project is to be generic, but that's how it was designed to be used.

A GameService takes care of business logic and doesn't live inside any MonoBehaviour class. It's a reference loaded by Modulate.Main and it works in pair with a Manager. The Manager has a reference to it's service and they can freely comunicate interally. It's highly advisable to make use of async libraries like Unitask and make it async for most purposes.

The ManagerContainer is a Monobehaviour class that hosts all the Managers. It has a custom inspector designed to nest every manager.



![ModulateMap drawio](https://github.com/user-attachments/assets/7c2c9a98-45cb-495f-a942-62ec9222b607)





Before installing, please import this package using Gir URL:

https://github.com/AlbertoVosgerau/DDElements.git#0.1.4

Then use this link to add this package:

https://github.com/AlbertoVosgerau/Modulate.git

OR

https://github.com/AlbertoVosgerau/Modulate.git#versionNumber

Latest version:

https://github.com/AlbertoVosgerau/Modulate.git#0.1.0
