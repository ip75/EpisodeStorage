#include <iostream>

#define ZMQ_BUILD_DRAFT_API
#include <zmq.h>
#include <zmq.hpp>
#include <nlohmann/json.hpp>
#include <boost/endian/conversion.hpp>

int main(int argc, char** argv)
{
	int64_t episode_id = 0;
    zmq::context_t context(1);

    //  Socket to talk to server
    zmq::socket_t dealer (context, ZMQ_DEALER);

    const auto ident = "DETECTOR 1";
    dealer.setsockopt(ZMQ_IDENTITY, ident, strlen (ident));
    dealer.connect("tcp://127.0.0.1:10000");

	
    const auto msg = R"({ "Command": "store_file",
						  "FileName": "buffer.test.log",
						  "Data": "dGltZXN0YW1wOiAxMzIxNTEwNDYzMDMyNTY5OTAgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4zMjU2OTkNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAzMjM0MTgxIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMzIzNDE4DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMzE1NTI1NyB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjMxNTUyNQ0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDMxMzExNzAgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4zMTMxMTcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAyOTk2MTEwIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMjk5NjExDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMjg4MjQyNiB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjI4ODI0Mg0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDI4NTUzNzIgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4yODU1MzcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAyNzE1MjA0IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMjcxNTIwDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMjU4OTAyNiB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjI1ODkwMg0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDI0NzgyMzYgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4yNDc4MjMNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAyNDUyOTg4IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMjQ1Mjk4DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMjE2NDc1MyB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjIxNjQ3NQ0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDIxMzkwNzAgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4yMTM5MDcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAxODkxMDY0IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMTg5MTA2DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMTg2MzY2NSB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjE4NjM2Ng0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDE3MTgxNzYgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4xNzE4MTcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAxNjQ2MTA1IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMTY0NjEwDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMTUyNjYyMCB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjE1MjY2Mg0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDE0OTg1NzcgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4xNDk4NTcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAxMzQ2MzU4IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMTM0NjM1DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMTI4MzAyMyB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjEyODMwMg0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDExOTc2NzkgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4xMTk3NjcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAxMTcxNDY3IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMTE3MTQ2DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMTEwNjk2MSB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjExMDY5Ng0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDA5OTQ1NjkgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4wOTk0NTYNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAwOTUyMzQwIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMDk1MjM0DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMDkyNDI2NyB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjA5MjQyNg0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDA4Mzc0NzIgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4wODM3NDcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAwNzY5MjcxIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMDc2OTI3DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMDY4NzExMSB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjA2ODcxMQ0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDA2MzQ2MDAgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4wNjM0NjANCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAwNTIxODM3IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMDUyMTgzDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMDQ1MjYxNiB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjA0NTI2MQ0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDAzODUyODYgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4wMzg1MjgNCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAwMjk2ODExIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMDI5NjgxDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMDIyMzA5NyB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjAyMjMwOQ0KdGltZXN0YW1wOiAxMzIxNTEwNDYzMDAxMzA4MDAgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo1MC4wMTMwODANCnRpbWVzdGFtcDogMTMyMTUxMDQ2MzAwMTA3NTAyIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NTAuMDEwNzUwDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjMwMDAxNzY2NyB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjUwLjAwMTc2Ng0KdGltZXN0YW1wOiAxMzIxNTEwNDYyOTk5OTQzNjAgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo0OS45OTk0MzYNCnRpbWVzdGFtcDogMTMyMTUxMDQ2Mjk5ODc4ODk1IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NDkuOTg3ODg5DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjI5OTgwOTk4MSB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjQ5Ljk4MDk5OA0KdGltZXN0YW1wOiAxMzIxNTEwNDYyOTk3Njk1NzEgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo0OS45NzY5NTcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2Mjk5NjkwODYzIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NDkuOTY5MDg2DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjI5OTYzNzA2OCB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjQ5Ljk2MzcwNg0KdGltZXN0YW1wOiAxMzIxNTEwNDYyOTk1MTg4MTYgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo0OS45NTE4ODENCnRpbWVzdGFtcDogMTMyMTUxMDQ2Mjk5Mzg3NDQ2IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NDkuOTM4NzQ0DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjI5OTM1NTY3NSB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjQ5LjkzNTU2Nw0KdGltZXN0YW1wOiAxMzIxNTEwNDYyOTkyOTczNDQgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo0OS45Mjk3MzQNCnRpbWVzdGFtcDogMTMyMTUxMDQ2Mjk5MTkxMDM0IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NDkuOTE5MTAzDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjI5OTExMzAxMyB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjQ5LjkxMTMwMQ0KdGltZXN0YW1wOiAxMzIxNTEwNDYyOTkwODkzMDYgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo0OS45MDg5MzANCnRpbWVzdGFtcDogMTMyMTUxMDQ2Mjk5MDE4MzAzIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NDkuOTAxODMwDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjI5ODkxMTI4OSB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjQ5Ljg5MTEyOA0KdGltZXN0YW1wOiAxMzIxNTEwNDYyOTg4ODcwNzYgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo0OS44ODg3MDcNCnRpbWVzdGFtcDogMTMyMTUxMDQ2Mjk4ODEyNzgwIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NDkuODgxMjc4DQp0aW1lc3RhbXA6IDEzMjE1MTA0NjI5ODc3NjI2MCB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjQ5Ljg3NzYyNg0KdGltZXN0YW1wOiAxMzIxNTEwNDYyOTg2ODk3OTIgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo0OS44Njg5NzkNCnRpbWVzdGFtcDogMTMyMTUxMDQ2Mjk4NjI1NTM5IHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NDkuODYyNTUzDQp0aW1lc3RhbXA6IDEzMjE1MTA0NjI5ODU3MjgxMiB0aW1lOiAyMDE5LTEwLTA5VDE0OjIzOjQ5Ljg1NzI4MQ0KdGltZXN0YW1wOiAxMzIxNTEwNDYyOTg0ODg4MTkgdGltZTogMjAxOS0xMC0wOVQxNDoyMzo0OS44NDg4ODENCnRpbWVzdGFtcDogMTMyMTUxMDQ2Mjk4Mzk3NDIwIHRpbWU6IDIwMTktMTAtMDlUMTQ6MjM6NDkuODM5NzQyDQo="
					})"_json;

    for (auto update_nbr = 0; update_nbr < 100; update_nbr++) {
    	
        zmq::message_t command(msg.dump().data(), msg.dump().size()), result;
	    std::cout << "Sending file to server: " << msg["FileName"].get<std::string>() << std::endl;

    	command.set_routing_id(111);
    	command.set_group(ident);

		auto command_name(msg["Command"].get<std::string>());
		zmq::message_t command_name_message(reinterpret_cast<const void*>(command_name.data()), command_name.size());
    	episode_id = boost::endian::endian_reverse(episode_id);
    	zmq::message_t episode_id_message(reinterpret_cast<void*>(&episode_id), sizeof(episode_id));

    	
    	
    	dealer.send(episode_id_message, zmq::send_flags::sndmore);
    	dealer.send(command_name_message, zmq::send_flags::sndmore);
    	dealer.send(command, zmq::send_flags::none);


		do
		{
	        auto rc = dealer.recv(result, zmq::recv_flags::none);

			if(result.size() == sizeof(int64_t))  // we received episode ID, first frame
			{
				episode_id = boost::endian::endian_reverse(*reinterpret_cast<int64_t*>(result.data())); // *reinterpret_cast<int64_t*>(result.data());
			}
			std::cout << "Episode ID: " << episode_id << std::endl;


			
			if(result.size() > sizeof(int64_t))   // we received response JSON, second frame from router
			{
				std::string json(reinterpret_cast<char*>(result.data()), result.size());
    			std::cout << "Command response: " << json << std::endl; // result.str()
			}

	/*
	 *    	does not work
    		std::cout << "ZMQ_MSG_PROPERTY_SOCKET_TYPE: " << result.gets(ZMQ_MSG_PROPERTY_SOCKET_TYPE) << std::endl;
    		std::cout << "ZMQ_MSG_PROPERTY_ROUTING_ID: " << result.gets(ZMQ_MSG_PROPERTY_ROUTING_ID) << std::endl;
    		std::cout << "ZMQ_MSG_PROPERTY_USER_ID: " << result.gets(ZMQ_MSG_PROPERTY_USER_ID) << std::endl;
    		std::cout << "ZMQ_MSG_PROPERTY_PEER_ADDRESS: " << result.gets(ZMQ_MSG_PROPERTY_PEER_ADDRESS) << std::endl;
	*/
	   		std::cout << "ZMQ_MORE: " << result.get(ZMQ_MORE) << std::endl;
    		std::cout << "ZMQ_SHARED: " << result.get(ZMQ_SHARED) << std::endl;
    		std::cout << "Routing ID: " << result.routing_id() << std::endl;
    		std::cout << "more: " << result.more() << std::endl;
    		std::cout << "group: " << result.group() << std::endl;
		} while (result.more());  // iterate receiving frames from router

//        std::istringstream iss(static_cast<char*>(command_result.data()));
    }
    return 0;
}
