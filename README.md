RabbitMQ .Net Core client demos
===============================

This set of demos have the purpose of describing some basic and most commonly used RabbitMQ features.

The set of demos utilize .Net Core platform, C# programming language and RabbitMQ.Client library.

Prerequisites
-------------

First, you should have the latest RabbitMQ installed.

Please check out RMQ downloads page and follow the instructions provided:

http://www.rabbitmq.com/download.html

Before running the **Delayed** demo, you should also download the **rabbitmq_delayed_message_exchange** community plugin, then put it into RMQ plugins directory and enable it.

http://www.rabbitmq.com/community-plugins.html

````rabbitmq-plugins enable rabbitmq_delayed_message_exchange````

**Windows users:** 

Before installing the RMQ message broker, you should first install latest 64-bit Erlang/OTP platform.

https://www.erlang.org/downloads

Don't forget to add RMQ sbin dir to your PATH, you will then have the ability to use RMQ command-line tools.

Default RMQ tools directory could be located at:

````C:\Program Files\RabbitMQ Server\rabbitmq_server-3.7.8\sbin````

Repository structure
--------------------

This repository has 5 solutions that correspond to the following demos:

### RabbitDemo.SimpleQueue

The demo utilizes basic RMQ tasks, such as the declaration of a queue, the subscribition to the messages, and also the built-in RMQ tracing ability.

**To be able to receive tracer messages, you should first enable RMQ tracing.**

````rabbitmqctl trace_on````

### RabbitDemo.Fanout

The demo utilizes ````fanout```` exchange type, which is used to send the messages to all of the consumers available.

### RabbitDemo.Delayed

The demo utilizes **rabbitmq_delayed_message_exchange** plugin for the purpose of delaying the message delivery.

### RabbitDemo.Durable

The demo utilizes durable exchanges, durable queues and persistent messages, which are used to establish message delivery guarantees.

### RabbitDemo.Rpc

The demo describes the process of utilizing the message broker for the purpose of making request/response RPC client-server architecture.

Building and running
--------------------

The most convenient way to build and run the solutions is to either use VisualStudio 2017 or Visual Studio Code.

Although all the projects available could of course be built and run using the dotnet command.