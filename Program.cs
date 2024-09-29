//*****************************************************************************
//** 432. All O'one Data Structure    leetcode                               **
//*****************************************************************************


typedef struct Node {
    int count;
    struct Node* next;
    struct Node* prev;
    char** words;  // array of strings to store words
    int word_count; // to track number of words in the set
    int word_capacity; // to manage the capacity of the words array
} Node;

typedef struct {
    Node* head;
    Node* tail;
    Node** hashMap; // array to store the nodes corresponding to each key
    int mapSize;
} AllOne;

// Helper function to resize the words array in a Node
void resizeWordsArray(Node* node) {
    node->word_capacity *= 2;
    node->words = (char**)realloc(node->words, sizeof(char*) * node->word_capacity);
}

// Function to create a new node
Node* createNode(int count, const char* key) {
    Node* newNode = (Node*)malloc(sizeof(Node));
    newNode->count = count;
    newNode->next = NULL;
    newNode->prev = NULL;
    newNode->word_capacity = 10;
    newNode->words = (char**)malloc(sizeof(char*) * newNode->word_capacity);
    newNode->word_count = 1;
    newNode->words[0] = strdup(key); // add the key to the words set
    return newNode;
}

// Function to initialize the AllOne structure
AllOne* allOneCreate() {
    AllOne* obj = (AllOne*)malloc(sizeof(AllOne));
    
    // Initialize head and tail nodes
    obj->head = createNode(-1, "");
    obj->tail = createNode(-1, "");
    
    obj->head->next = obj->tail;
    obj->tail->prev = obj->head;

    // Initialize hashMap with a fixed size (you can adjust this as necessary)
    obj->mapSize = 1000;  // Assuming max 1000 unique keys for simplicity
    obj->hashMap = (Node**)calloc(obj->mapSize, sizeof(Node*));
    
    return obj;
}

// Helper function to add a node between two nodes
void addNode(AllOne* obj, Node* prevNode, Node* newNode, Node* nextNode) {
    prevNode->next = newNode;
    nextNode->prev = newNode;
    newNode->prev = prevNode;
    newNode->next = nextNode;
}

// Helper function to remove a node from the doubly linked list
void removeNode(Node* node) {
    node->prev->next = node->next;
    node->next->prev = node->prev;
    for (int i = 0; i < node->word_count; i++) {
        free(node->words[i]);
    }
    free(node->words);
    free(node);
}

// Hash function to generate index from string
int getHashIndex(AllOne* obj, const char* key) {
    unsigned long hash = 5381;
    int c;
    while ((c = *key++))
        hash = ((hash << 5) + hash) + c;
    return hash % obj->mapSize;
}

// Helper function to insert a word into a node's words array lexicographically
void insertWord(Node* node, const char* word) {
    if (node->word_count >= node->word_capacity) {
        resizeWordsArray(node);
    }

    int i;
    for (i = node->word_count - 1; i >= 0 && strcmp(node->words[i], word) > 0; i--) {
        node->words[i + 1] = node->words[i];
    }
    node->words[i + 1] = strdup(word);
    node->word_count++;
}

// Helper function to remove a word from a node's words array
void removeWord(Node* node, const char* word) {
    int i;
    for (i = 0; i < node->word_count; i++) {
        if (strcmp(node->words[i], word) == 0) {
            free(node->words[i]);
            break;
        }
    }
    for (; i < node->word_count - 1; i++) {
        node->words[i] = node->words[i + 1];
    }
    node->word_count--;
}

// Increment the count of a key
void allOneInc(AllOne* obj, const char* key) {
    int idx = getHashIndex(obj, key);
    
    if (obj->hashMap[idx] == NULL) {
        // If key is not present, add it with count 1
        if (obj->head->next->count == 1) {
            insertWord(obj->head->next, key);
            obj->hashMap[idx] = obj->head->next;
        } else {
            Node* newNode = createNode(1, key);
            addNode(obj, obj->head, newNode, obj->head->next);
            obj->hashMap[idx] = newNode;
        }
    } else {
        // If key is present, increment its count
        Node* node = obj->hashMap[idx];
        removeWord(node, key);
        
        if (node->next->count == node->count + 1) {
            insertWord(node->next, key);
            obj->hashMap[idx] = node->next;
        } else {
            Node* newNode = createNode(node->count + 1, key);
            addNode(obj, node, newNode, node->next);
            obj->hashMap[idx] = newNode;
        }
        
        if (node->word_count == 0) {
            removeNode(node);
        }
    }
}

// Decrement the count of a key
void allOneDec(AllOne* obj, const char* key) {
    int idx = getHashIndex(obj, key);
    Node* node = obj->hashMap[idx];
    
    // Remove key from the current node
    removeWord(node, key);
    
    if (node->prev->count == node->count - 1) {
        insertWord(node->prev, key);
        obj->hashMap[idx] = node->prev;
    } else if (node->count - 1 > 0) {
        Node* newNode = createNode(node->count - 1, key);
        addNode(obj, node->prev, newNode, node);
        obj->hashMap[idx] = newNode;
    } else {
        obj->hashMap[idx] = NULL;
    }
    
    if (node->word_count == 0) {
        removeNode(node);
    }
}

// Get the key with the maximum count
char* allOneGetMaxKey(AllOne* obj) {
    if (obj->tail->prev == obj->head) {
        return "";
    }
    return obj->tail->prev->words[0];
}

// Get the key with the minimum count (lexicographically smallest word)
char* allOneGetMinKey(AllOne* obj) {
    if (obj->head->next == obj->tail) {
        return "";
    }
    return obj->head->next->words[0];
}

// Free the memory allocated for AllOne
void allOneFree(AllOne* obj) {
    Node* curr = obj->head;
    while (curr != NULL) {
        Node* next = curr->next;
        for (int i = 0; i < curr->word_count; i++) {
            free(curr->words[i]);
        }
        free(curr->words);
        free(curr);
        curr = next;
    }
    free(obj->hashMap);
    free(obj);
}
/**
 * Your AllOne struct will be instantiated and called as such:
 * AllOne* obj = allOneCreate();
 * allOneInc(obj, key);
 
 * allOneDec(obj, key);
 
 * char* param_3 = allOneGetMaxKey(obj);
 
 * char* param_4 = allOneGetMinKey(obj);
 
 * allOneFree(obj);
*/