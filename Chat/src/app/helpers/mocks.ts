export const MARLEY = { id: 1, fullName: "Bot Marley", userName: "Marley", type: 1}
export const ME = { id: 2, fullName: "Ksenia Sadrina", userName: "Makaka", type: 0}

export const MESSAGES = [
  { id: 1, sender: MARLEY, text: "Hi there, welcome to our training. If you have questions feel free to ask me.", sessionId: 1, timestamp: new Date("2019-04-06T10:52:15")},
  { id: 2, sender: ME, text: "Hi, what kind of training is it?", sessionId: 1, timestamp: new Date("2019-04-06T10:52:50")},
  { id: 3, sender: MARLEY, text: "this is an apache shutdown training.", sessionId: 1, timestamp: new Date("2019-04-06T10:54:15")},
  { id: 4, sender: ME, text: "cool, which tool should I use to start?", sessionId: 1, timestamp: new Date("2019-04-06T10:54:15")},
  { id: 5, sender: MARLEY, text: "Try using zenoss", sessionId: 1, timestamp: new Date("2019-04-06T10:54:15")},
  { id: 6, sender: ME, text: "Ok, i'll try.", sessionId: 1, timestamp: new Date("2019-04-06T10:54:15")},
  { id: 7, sender: ME, text: "thanks for the help", sessionId: 1, timestamp: new Date("2019-04-06T10:54:15")},
  { id: 8, sender: MARLEY, text: "You are welcome", sessionId: 1, timestamp: new Date("2019-04-06T10:54:15")}
];

export const SQLINJECTION = {  id: 1, name: "SQLInjection", description: "SQL injection is a code injection technique that might destroy your database.SQL injection is one of the most common web hacking techniques.SQL injection is the placement of malicious code in SQL statements, via web page input."};
export const APACHE = { id: 2, name: "Apache Shutdown", description: "Slowloris is a type of denial of service attack tool invented by Robert 'RSnake' Hansen which allows a single machine to take down another machine's web server with minimal bandwidth and side effects on unrelated services and ports."};  
export const TROJAN = { id: 3, name: "Trojan", description: "any malicious computer program which misleads users of its true intent. The term is derived from the Ancient Greek story of the deceptive wooden horse that led to the fall of the city of."};

export const SCENARIOS = [ SQLINJECTION, APACHE, TROJAN];

export const TRAINING = [
  { id: 1, name: 'Team AAA', scenario: SQLINJECTION, state: 1 },
  { id: 2, name: 'Apache Shutdown #1', scenario: APACHE, state: 2 },
  { id: 3, name: 'We are Trojan!', scenario: TROJAN, state: 1 },
  { id: 4, name: 'Fun with sql injection', scenario: SQLINJECTION, state: 3 },
];